import { Component, OnInit, inject } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from './../../../services/auth';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import {
  ItemDto,
  ItemUpdateDto,
  DiscountDto,
  PromoCodeDto,
} from '../../../models/DTO.model';

@Component({
  selector: 'app-menu-management',
  templateUrl: './menu-management.html',
  styleUrls: ['./menu-management.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    MatTableModule,
  ],
})
export class MenuManagement implements OnInit {
  restaurantId = '';

  categoryFilter: string = '';
  nameFilter: string = '';

  items: ItemDto[] = [];
  filteredItems: ItemDto[] = [];
  categories: string[] = [];
  imagePreviewUrl: string | null = null;

  Discounts: DiscountDto[] = [];

  promoCodes: PromoCodeDto[] = [];
  filteredPromoCodes: PromoCodeDto[] = [];
  promoCodeFilter = '';

  editItemId: string | null = null; // Track currently edited item for edit mode if needed
  promoCodeEditMode = false;
  editedPromoCodeId: string | null = null;
  discountEditMode = false;

  addItemForm: FormGroup;
  discountForm: FormGroup;
  promoCodeForm: FormGroup;

  private http = inject(HttpClient);
  private snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);

  private baseUrl = 'http://localhost:5000/api';

  constructor() {
    this.addItemForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      discountedPrice: [''],
      category: ['', Validators.required],
      isAvailable: [true],
      imageFile: [null],
      imageUrl: [null],
    });

    this.discountForm = this.fb.group({
      discountID: [null],
      itemID: ['', Validators.required],
      itemName: [''],
      percentage: [
        0,
        [Validators.required, Validators.min(0), Validators.max(100)],
      ],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });

    this.promoCodeForm = this.fb.group({
      promoCodeID: [null],
      Code: ['', Validators.required],
      DiscountPercentage: [
        0,
        [Validators.required, Validators.min(0), Validators.max(100)],
      ],
      IsFreeDelivery: [false],
      ExpiryDate: ['', Validators.required],
      UsageLimit: [1, [Validators.required, Validators.min(1)]],
    });
  }

  ngOnInit(): void {
    this.restaurantId = this.authService.getUserId() || '';
    console.log('Restaurant ID:', this.restaurantId);
    if (!this.restaurantId) {
      this.snackBar.open(
        'User/Restaurant ID missing. Please login.',
        undefined,
        { duration: 3000 }
      );
      return;
    }

    setTimeout(() => {
      this.loadItems();
      this.loadDiscounts();
      this.loadPromoCodes();
    });

    this.discountForm
      .get('itemID')
      ?.valueChanges.subscribe((selectedItemId) => {
        const foundItem = this.items.find(
          (item) => item.itemID === selectedItemId
        );
        // Patch the found item's name or empty string
        this.discountForm.patchValue(
          { itemName: foundItem ? foundItem.name : '' },
          { emitEvent: false }
        );
      });
  }

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.isLoggedIn();
    console.log('Token Info', token);
    return token
      ? new HttpHeaders({ Authorization: `Bearer ${token}` })
      : new HttpHeaders();
  }

  public getImageUrl(imageFile?: string): string {
    return this.authService.getImageUrl(imageFile);
  }
  // ===== Items =====

  loadItems(): void {
    const headers = this.getAuthHeaders();

    if (this.categoryFilter) {
      const query = `category=${encodeURIComponent(this.categoryFilter)}`;
      this.http
        .get<ItemDto[]>(
          `${this.baseUrl}/item/${this.restaurantId}/items/bycategory?${query}`,
          { headers }
        )
        .subscribe({
          next: (items) => {
            this.items = Array.isArray(items) ? items : [];
            this.extractCategories();
            this.applyItemFilters();
            this.setupItemIdValueChangesSubscription();
          },
          error: () => {
            this.snackBar.open('Failed to load items by category', undefined, {
              duration: 3000,
            });
          },
        });
    } else {
      this.http
        .get<ItemDto[]>(`${this.baseUrl}/item/items`, { headers })
        .subscribe({
          next: (res) => {
            console.log(res);
            let itemsArray: ItemDto[] = [];
            if (Array.isArray(res)) {
              itemsArray = res;
            } else if (res && Array.isArray((res as any).$values)) {
              itemsArray = (res as any).$values;
              console.log('itemsArray', itemsArray);
            }

            this.items = itemsArray;
            console.log('itemsArray:', itemsArray);
            console.log('this.items:', this.items);
            this.extractCategories();
            this.applyItemFilters();
            this.setupItemIdValueChangesSubscription();
          },
          error: () => {
            this.snackBar.open('Failed to load all items', undefined, {
              duration: 3000,
            });
          },
        });
    }
  }

  private itemIdSubscriptionSet = false; // flag to prevent multiple subscriptions

  private setupItemIdValueChangesSubscription(): void {
    if (this.itemIdSubscriptionSet) return; // prevent multiple subscriptions

    this.discountForm
      .get('itemID')
      ?.valueChanges.subscribe((selectedItemId) => {
        const foundItem = this.items.find(
          (item) => item.itemID === selectedItemId
        );
        this.discountForm.patchValue(
          { itemName: foundItem ? foundItem.name : '' },
          { emitEvent: false }
        );
      });

    this.itemIdSubscriptionSet = true;
  }

  onImageFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.addItemForm.patchValue({
        imageFile: file,
      });
      // If you want to trigger validation or mark as touched
      this.addItemForm.get('imageFile')?.updateValueAndValidity();
      this.getImageUrl('imageFile');
    }
  }

  extractCategories(): void {
    this.categories = Array.from(
      new Set(this.items.map((i) => i.category ?? ''))
    );
    console.log('Categories extracted:', this.categories);
  }

  applyItemFilters(): void {
    const nameFilter = this.nameFilter ? this.nameFilter.toLowerCase() : '';
    const categoryFilter = this.categoryFilter ?? '';
    this.filteredItems = this.items.filter(
      (item) =>
        (!nameFilter || item.name?.toLowerCase().includes(nameFilter)) &&
        (!categoryFilter || item.category === categoryFilter)
    );
  }

  onCategoryChange(category: string): void {
    this.categoryFilter = category;
    this.applyItemFilters();
  }

  onNameFilterChange(name: string): void {
    this.nameFilter = name;
    this.applyItemFilters();
  }

  // submit handler to handle both adding and editing
  submitItem(): void {
    if (this.addItemForm.invalid || !this.restaurantId) {
      this.snackBar.open(
        'Please fill in all required item information correctly.',
        undefined,
        { duration: 3000 }
      );
      return;
    }

    const formValue = this.addItemForm.value;
    const formData = new FormData();

    formData.append('Name', formValue.name);
    formData.append('Description', formValue.description || '');
    formData.append('Price', formValue.price.toString());
    formData.append('DiscountedPrice', formValue.discountedPrice.toString());
    formData.append('Category', formValue.category);
    formData.append('IsAvailable', formValue.isAvailable ? 'true' : 'false');
    formData.append('RestaurantID', this.restaurantId);

    // Append image file if any
    if (formValue.imageFile) {
      formData.append('ImageFile', formValue.imageFile);
    }

    const headers = this.getAuthHeaders();

    // Check if editing or adding new
    if (this.editItemId) {
      formData.append('ItemID', this.editItemId);
      console.log('Submitting item, editItemId:', this.editItemId);
      // Edit mode — send PUT request with FormData
      this.http
        .put(`${this.baseUrl}/item/items/${this.editItemId}`, formData, {
          headers,
        })
        .subscribe({
          next: () => {
            this.snackBar.open('Item updated successfully.', undefined, {
              duration: 3000,
            });
            this.addItemForm.reset({ isAvailable: true, price: 0 });
            this.editItemId = null; // Reset edit mode
            this.loadItems();
          },
          error: () => {
            this.snackBar.open('Failed to update item.', undefined, {
              duration: 3000,
            });
          },
        });
    } else {
      // Add - send POST request
      this.http
        .post(`${this.baseUrl}/item/${this.restaurantId}/items`, formData, {
          headers,
        })
        .subscribe({
          next: () => {
            this.snackBar.open('New item added successfully.', undefined, {
              duration: 3000,
            });
            this.addItemForm.reset({ isAvailable: true, price: 0 });
            this.loadItems();
          },
          error: () => {
            this.snackBar.open('Failed to add new item.', undefined, {
              duration: 3000,
            });
          },
        });
    }
  }

  // Called when user selects an item to edit -- just patches the form
  editItem(item: ItemUpdateDto): void {
    this.editItemId = item.itemID || null;
    this.imagePreviewUrl = this.getImageUrl(item.imageUrl);
    this.addItemForm.patchValue({
      name: item.name,
      description: item.description,
      price: item.price,
      discountedPrice: item.discountedPrice,
      category: item.category,
      isAvailable: item.isAvailable,
      imageFile: null, // Do not patch the file input directly
      imageUrl: item.imageUrl,
    });
    if (typeof window !== 'undefined') {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }

  }

  cancelEditItem(): void {
    this.editItemId = null;
    this.imagePreviewUrl = null;
    this.addItemForm.reset({
      name: '',
      description: '',
      price: 0,
      category: '',
      isAvailable: true,
      imageFile: null,
      imageUrl: null,
    });
    if (typeof window !== 'undefined') {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }

  }

  deleteItem(item: ItemDto): void {
    if (!item.itemID) {
      this.snackBar.open('Item ID missing.', undefined, { duration: 3000 });
      return;
    }
    if (!confirm(`Are you sure you want to delete item "${item.name}"?`)) {
      return;
    }

    this.http
      .delete(`${this.baseUrl}/item/items/${item.itemID}`, {
        headers: this.getAuthHeaders(),
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Item deleted successfully.', undefined, {
            duration: 3000,
          });
          this.loadItems();
        },
        error: () =>
          this.snackBar.open('Failed to delete item.', undefined, {
            duration: 3000,
          }),
      });
  }

  // ===== Discounts =====

  loadDiscounts(): void {
    this.http
      .get<DiscountDto[]>(
        `${this.baseUrl}/discount/${this.restaurantId}/discounts`,
        { headers: this.getAuthHeaders() }
      )
      .subscribe({
        next: (res) => {
          console.log(this.restaurantId);
          console.log(res);
          let discountsArray: DiscountDto[] = [];
          if (Array.isArray(res)) {
            discountsArray = res;
          } else if (res && Array.isArray((res as any).$values)) {
            discountsArray = (res as any).$values;
          }
          this.Discounts = discountsArray;
          console.log('discountsArray:', discountsArray);
          console.log('this.Discounts:', this.Discounts);
        },
        error: (err) => {
          if (err.status === 404)
            this.snackBar.open(
              'No discounts endpoint found. Backend not implemented.',
              undefined,
              { duration: 3000 }
            );
          else
            this.snackBar.open('Failed to load discounts', undefined, {
              duration: 3000,
            });
        },
      });
  }

  submitDiscount(): void {
    if (this.discountForm.invalid || !this.restaurantId) {
      this.snackBar.open(
        'Provide all required discount data correctly.',
        undefined,
        { duration: 3000 }
      );
      return;
    }

    // Extract form value
    const formValue = this.discountForm.value;

    // Build DTO - include all needed properties your backend expects
    // Including itemId, itemName, percentage, startDate, endDate
    const dto: DiscountDto = {
      discountID: formValue.discountID, // could be null for add
      itemID: formValue.itemID,
      itemName: formValue.itemName,
      percentage: formValue.percentage,
      startDate: formValue.startDate,
      endDate: formValue.endDate,
    };
    console.log('Discountdto', dto);

    // Determine API call: PUT for update, POST for add
    let request$;
    if (formValue.discountID !== null && formValue.discountID !== undefined) {
      // Update existing discount by ID
      request$ = this.http.put(
        `${this.baseUrl}/discount/discounts/${formValue.discountID}`,
        dto,
        { headers: this.getAuthHeaders() }
      );
    } else {
      // Add new discount, with restaurantId and itemId in URL as your API expects
      request$ = this.http.post(
        `${this.baseUrl}/discount/${this.restaurantId}/discounts/${formValue.itemID}`,
        dto,
        {
          headers: this.getAuthHeaders(),
        }
      );
    }

    request$.subscribe({
      next: () => {
        this.snackBar.open(
          formValue.discountID ? 'Discount updated' : 'Discount added',
          undefined,
          { duration: 3000 }
        );
        this.discountForm.reset();
        // Clear discountId to exit edit mode
        this.discountForm.patchValue({ discountID: null });
        this.loadDiscounts();
      },
      error: () =>
        this.snackBar.open('Failed to save discount', undefined, {
          duration: 3000,
        }),
    });
  }

  editDiscount(discount: DiscountDto): void {
    console.log('discount:', discount);
    this.discountForm.patchValue({
      discountID: discount.discountID,
      itemID: discount.itemID,
      itemName: discount.itemName,
      percentage: discount.percentage,
      startDate: this.formatDateForInput(discount.startDate),
      endDate: this.formatDateForInput(discount.endDate),
    });
    if (typeof window !== 'undefined') {
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  deleteDiscount(discount: DiscountDto): void {
    console.log('discount to delete:', discount);
    console.log('discount.discountID', discount.discountID);
    if (discount.discountID === null || discount.discountID === undefined) {
      this.snackBar.open('Discount ID missing.', undefined, { duration: 3000 });
      return;
    }
    if (!confirm(`Delete discount #${discount.discountID}?`)) return;
    console.log('Deleting discount with ID:', discount.discountID); // For debugging
    this.http
      .delete(`${this.baseUrl}/discount/discounts/${discount.discountID}`, {
        headers: this.getAuthHeaders(),
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Discount deleted', undefined, { duration: 3000 });
          this.loadDiscounts();
        },
        error: (err) => {
          console.error('Delete discount error:', err);
          this.snackBar.open('Failed to delete discount', undefined, {
            duration: 3000,
          });
        },
      });
  }

  cancelDiscountEdit(): void {
    // Reset the discount form and clear edit state
    this.discountForm.reset();

    // Optionally reset fields to default values, e.g.:
    this.discountForm.patchValue({
      discountID: null,
      itemID: '',
      itemName: '',
      percentage: 0,
      startDate: '',
      endDate: '',
    });

    // If you want, you can track an edit mode flag and reset it here, for example:
    // this.discountEditMode = false;
  }

  // Helper: format ISO string to yyyy-MM-dd for input[type=date]
  private formatDateForInput(date: string | Date): string {
    if (!date) return '';
    return new Date(date).toISOString().substring(0, 10);
  }

  // ===== Promo Codes =====

  // Called when user types in filter input for promo codes
  onPromoCodeFilterChange(value: string): void {
    this.promoCodeFilter = value;
    // Optional: debouncing can improve UX for large data sets.
    this.applyPromoCodeFilter();
  }

  // Applies filter based on current promoCodeFilter string to filteredPromoCodes
  applyPromoCodeFilter(): void {
    const filter = this.promoCodeFilter.toLowerCase().trim();
    this.filteredPromoCodes = this.promoCodes.filter(
      (pc) => !filter || (pc.code && pc.code.toLowerCase().includes(filter))
    );
  }

  // Loads promo codes from backend API, handles different response formats
  loadPromoCodes(): void {
    this.http
      .get<any>(`${this.baseUrl}/promoCode/${this.restaurantId}/promocodes`, {
        headers: this.getAuthHeaders(),
      })
      .subscribe({
        next: (res) => {
          console.log('promo code res', res);
          let codesArray: PromoCodeDto[] = [];
          if (Array.isArray(res)) {
            codesArray = res;
          } else if (res && Array.isArray(res.$values)) {
            codesArray = res.$values;
          }
          this.promoCodes = codesArray;
          this.applyPromoCodeFilter();
        },
        error: (err) => {
          const msg =
            err.status === 500
              ? 'Server error loading promo codes'
              : 'Promo code endpoint failed';
          this.snackBar.open(msg, undefined, { duration: 3000 });
        },
      });
  }

  // Submits new or edited promo code to the backend
  submitPromoCode(): void {
    if (this.promoCodeForm.invalid || !this.restaurantId) {
      this.snackBar.open(
        'Provide all required promo code info correctly.',
        undefined,
        { duration: 3000 }
      );
      return;
    }

    const rawValue = this.promoCodeForm.value;
    const promoCodeId = rawValue.promoCodeID;

    // Normalize DTO to your backend expectations
    const dto: PromoCodeDto = {
      code: rawValue.Code,
      discountPercentage: rawValue.DiscountPercentage,
      isFreeDelivery: rawValue.IsFreeDelivery,
      expiryDate: new Date(rawValue.ExpiryDate).toISOString(),
      usageLimit: rawValue.UsageLimit,
      promoCodeID: promoCodeId,
      issuedByType: rawValue.IssuedByType,
    };

    const base = `${this.baseUrl}/promoCode/${this.restaurantId}/promocodes`;

    const request$ =
      this.promoCodeEditMode && promoCodeId
        ? this.http.put(`${base}/${promoCodeId}`, dto, {
            headers: this.getAuthHeaders(),
          })
        : this.http.post(base, dto, { headers: this.getAuthHeaders() });

    request$.subscribe({
      next: () => {
        this.snackBar.open(
          this.promoCodeEditMode ? 'Promo code updated' : 'Promo code added',
          undefined,
          { duration: 3000 }
        );
        this.cancelPromoCodeEdit(); // Clear form and exit edit mode
        this.loadPromoCodes(); // Reload list fresh from backend
      },
      error: () =>
        this.snackBar.open('Failed to save promo code', undefined, {
          duration: 3000,
        }),
    });
  }

  // Edit a selected promo code: patches form and sets edit mode
  editPromoCode(promoCode: PromoCodeDto): void {
    this.promoCodeForm.patchValue({
      promoCodeID: promoCode.promoCodeID,
      Code: promoCode.code,
      DiscountPercentage: promoCode.discountPercentage,
      IsFreeDelivery: promoCode.isFreeDelivery,
      ExpiryDate: this.formatDateForInput(promoCode.expiryDate),
      UsageLimit: promoCode.usageLimit,
      IssuedByType: promoCode.issuedByType,
    });
    this.promoCodeEditMode = true;
    this.editedPromoCodeId = promoCode.promoCodeID
      ? String(promoCode.promoCodeID)
      : null;
  }

  // Cancel editing and reset form to defaults
  cancelPromoCodeEdit(): void {
    this.promoCodeEditMode = false;
    this.editedPromoCodeId = null;
    this.promoCodeForm.reset({
      Code: '',
      DiscountPercentage: 0,
      IsFreeDelivery: false,
      ExpiryDate: '',
      UsageLimit: 1,
      promoCodeID: null,
      IssuedByType: null,
    });
  }

  // Delete promo code after confirmation
  deletePromoCode(promoCode: PromoCodeDto): void {
    if (!confirm(`Delete promo code '${promoCode.code}'?`)) return;
    if (!this.restaurantId) {
      this.snackBar.open('Restaurant ID missing.', undefined, {
        duration: 3000,
      });
      return;
    }
    this.http
      .delete(
        `${this.baseUrl}/promoCode/${this.restaurantId}/promocodes/${promoCode.promoCodeID}`,
        { headers: this.getAuthHeaders() }
      )
      .subscribe({
        next: () => {
          this.snackBar.open('Promo code deleted', undefined, {
            duration: 3000,
          });
          this.loadPromoCodes();
        },
        error: () =>
          this.snackBar.open('Failed to delete promo code', undefined, {
            duration: 3000,
          }),
      });
  }
}
