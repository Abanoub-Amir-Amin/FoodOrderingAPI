import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';

import { CommonModule } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';

import { ItemDto, DiscountDto, PromoCodeDto } from '../../../models/DTO.model';

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
  Discounts: DiscountDto[] = [];
  filteredItems: any[] = [];
  categories: string[] = [];
  categoryFilter = '';
  nameFilter = '';
  addItemForm!: FormGroup;
  discountForm!: FormGroup;
  items: ItemDto[] = [];
  promoCodes: PromoCodeDto[] = [];
  promoCodeForm!: FormGroup;
  promoCodeFilter = '';
  filteredPromoCodes: any[] = [];
  promoCodeEditMode = false;
  editedPromoCodeId: string | null = null;

  private http = inject(HttpClient);
  private snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  private baseUrl = 'http://localhost:5000/api';

  ngOnInit(): void {
    this.restaurantId = localStorage.getItem('userId') || '';

    this.addItemForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      price: [0, [Validators.required, Validators.min(0)]],
      category: ['', Validators.required],
      isAvailable: [true],
      ImageFile: [null],
    });

    this.discountForm = this.fb.group({
      itemId: ['', Validators.required],
      percentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });

    this.promoCodeForm = this.fb.group({
      Code: ['', Validators.required],
      DiscountPercentage: [0, [Validators.required, Validators.min(0), Validators.max(100)]],
      IsFreeDelivery: [false],
      ExpiryDate: ['', Validators.required],
      UsageLimit: [1, [Validators.required, Validators.min(1)]],
    });

    this.loadItems();
    this.loadPromoCodes();
  }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('Token') || localStorage.getItem('token');
    return token ? new HttpHeaders({ Authorization: `Bearer ${token}` }) : new HttpHeaders();
  }

  loadItems(): void {
    if (!this.restaurantId) {
      this.snackBar.open('Restaurant ID missing.');
      return;
    }
    const category = this.categoryFilter ? `?category=${encodeURIComponent(this.categoryFilter)}` : '';
    this.http
      .get<any[]>(`${this.baseUrl}/restaurant/${this.restaurantId}/items/bycategory${category}`, { headers: this.getAuthHeaders() })
      .subscribe({
        next: items => {
          this.items = items;
          this.extractCategories();
          this.applyFilters();
        },
        error: () => this.snackBar.open(`Failed to load items${category ? ' by category' : ''}`),
      });
  }

  extractCategories(): void {
    const catSet = new Set(this.items.map(i => i.Category));
    this.categories = Array.from(catSet);
  }

  applyFilters(): void {
    this.filteredItems = this.items.filter(
      item =>
        (!this.nameFilter || item.Name.toLowerCase().includes(this.nameFilter.toLowerCase())) &&
        (!this.categoryFilter || item.Category === this.categoryFilter)
    );
  }

  onCategoryChange(category: string): void {
    this.categoryFilter = category;
    this.loadItems();
  }

  onNameFilterChange(name: string): void {
    this.nameFilter = name;
    this.applyFilters();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      this.addItemForm.patchValue({ ImageFile: file });
    }
  }

  submitNewItem(): void {
    if (this.addItemForm.invalid) {
      this.snackBar.open('Please fill in all required fields correctly');
      return;
    }

    const formValue = this.addItemForm.value;

    const formData = new FormData();
    formData.append('Name', formValue.name);
    formData.append('Description', formValue.description);
    formData.append('Price', formValue.price.toString());
    formData.append('Category', formValue.category);
    formData.append('IsAvailable', formValue.isAvailable ? 'true' : 'false');
    if (formValue.ImageFile) formData.append('ImageFile', formValue.ImageFile);

    this.http.post(`${this.baseUrl}/restaurant/${this.restaurantId}/items`, formData, { headers: this.getAuthHeaders() }).subscribe({
      next: () => {
        this.snackBar.open('Item added successfully');
        this.addItemForm.reset({ isAvailable: true });
        this.loadItems();
      },
      error: () => this.snackBar.open('Failed to add item'),
    });
  }

  editItem(item: any): void {
    this.snackBar.open(`Edit item feature is under construction for item ${item.Name}`);
  }

  deleteItem(item: any): void {
    if (!confirm(`Are you sure you want to delete item '${item.Name}'?`)) return;
    const itemId = item.ItemID ?? item.Name;

    this.http.delete(`${this.baseUrl}/restaurant/${this.restaurantId}/items/${itemId}`, { headers: this.getAuthHeaders() }).subscribe({
      next: () => {
        this.snackBar.open('Item deleted successfully');
        this.loadItems();
      },
      error: () => this.snackBar.open('Failed to delete item'),
    });
  }

  submitDiscount(): void {
    if (this.discountForm.invalid) {
      this.snackBar.open('Please fill in all discount fields correctly');
      return;
    }
    const formValue = this.discountForm.value;

    const dto = {
      ItemID: formValue.itemId,
      Percentage: formValue.percentage,
      StartDate: formValue.startDate,
      EndDate: formValue.endDate,
    };

    this.http
      .post(`${this.baseUrl}/restaurant/${this.restaurantId}/discounts/${dto.ItemID}`, dto, { headers: this.getAuthHeaders() })
      .subscribe({
        next: () => {
          this.snackBar.open('Discount added successfully');
          this.discountForm.reset();
          this.loadItems();
        },
        error: () => this.snackBar.open('Failed to add discount'),
      });
  }

  loadPromoCodes(): void {
    this.http.get<any[]>(`${this.baseUrl}/restaurant/${this.restaurantId}/promocodes`, { headers: this.getAuthHeaders() }).subscribe({
      next: codes => {
        this.promoCodes = codes;
        this.applyPromoCodeFilter();
      },
      error: () => this.snackBar.open('Failed to load promo codes'),
    });
  }

  applyPromoCodeFilter(): void {
    const filter = this.promoCodeFilter.toLowerCase();
    this.filteredPromoCodes = this.promoCodes.filter(p =>
      p.Code.toLowerCase().includes(filter)
    );
  }

  onPromoCodeFilterChange(code: string): void {
    this.promoCodeFilter = code;
    this.applyPromoCodeFilter();
  }

  submitPromoCode(): void {
    if (this.promoCodeForm.invalid) {
      this.snackBar.open('Please fill in all promo code fields correctly');
      return;
    }

    const dto = this.promoCodeForm.value;

    if (!this.promoCodeEditMode) {
      this.http.post(`${this.baseUrl}/restaurant/${this.restaurantId}/promocodes`, dto, { headers: this.getAuthHeaders() }).subscribe({
        next: () => {
          this.snackBar.open('Promo code added successfully');
          this.promoCodeForm.reset({ IsFreeDelivery: false, UsageLimit: 1, DiscountPercentage: 0 });
          this.loadPromoCodes();
        },
        error: () => this.snackBar.open('Failed to add promo code'),
      });
    } else if (this.editedPromoCodeId) {
      this.http
        .put(`${this.baseUrl}/restaurant/${this.restaurantId}/promocodes/${this.editedPromoCodeId}`, dto, { headers: this.getAuthHeaders() })
        .subscribe({
          next: () => {
            this.snackBar.open('Promo code updated successfully');
            this.promoCodeForm.reset({ IsFreeDelivery: false, UsageLimit: 1, DiscountPercentage: 0 });
            this.loadPromoCodes();
            this.promoCodeEditMode = false;
            this.editedPromoCodeId = null;
          },
          error: () => this.snackBar.open('Failed to update promo code'),
        });
    }
  }

  editPromoCode(promoCode: any): void {
    this.promoCodeForm.patchValue({
      Code: promoCode.Code,
      DiscountPercentage: promoCode.DiscountPercentage,
      IsFreeDelivery: promoCode.IsFreeDelivery,
      ExpiryDate: promoCode.ExpiryDate,
      UsageLimit: promoCode.UsageLimit,
    });
    this.promoCodeEditMode = true;
    this.editedPromoCodeId = promoCode.PromoCodeID as string;
  }

  cancelPromoCodeEdit(): void {
    this.promoCodeForm.reset({ IsFreeDelivery: false, UsageLimit: 1, DiscountPercentage: 0 });
    this.promoCodeEditMode = false;
    this.editedPromoCodeId = null;
  }

  deletePromoCode(promoCode: any): void {
    if (!confirm(`Delete promo code '${promoCode.Code}'?`)) return;

    const id = promoCode.PromoCodeID as string;
    this.http.delete(`${this.baseUrl}/restaurant/${this.restaurantId}/promocodes/${id}`, { headers: this.getAuthHeaders() }).subscribe({
      next: () => {
        this.snackBar.open('Promo code deleted');
        this.loadPromoCodes();
      },
      error: () => this.snackBar.open('Failed to delete promo code'),
    });
  }
}
