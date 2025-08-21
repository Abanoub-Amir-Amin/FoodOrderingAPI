import { Component, ElementRef, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { CustomerService } from '../../../services/customer/customer-service';
import { OrderDetailDTO, OrderItemDto, OrderViewDTO, StatusEnum } from '../../../models/DTO.model';
import { CommonModule } from '@angular/common';
import { Overlay, OverlayModule, OverlayRef } from '@angular/cdk/overlay';
import { PortalModule, TemplatePortal } from '@angular/cdk/portal';
import { StatustitlePipe } from '../../pipes/statustitle-pipe';
import { ReviewDTO, ReviewService } from '../../../services/review/review-service';
import { FormsModule } from '@angular/forms';
import { Rating } from "../../pages/rating/rating"; // ✅ استيراد FormsModule
import { NgxPrintModule } from 'ngx-print';

@Component({
  selector: 'app-customer-orders',
  imports: [CommonModule, OverlayModule, PortalModule, StatustitlePipe, FormsModule, Rating,NgxPrintModule],
  templateUrl: './customer-orders.html',
  styleUrl: './customer-orders.css',
})

export class CustomerOrders {
  StatusEnum=StatusEnum
  ViewStatusEnum = ViewStatusEnum
  SelectStatus:StatusEnum=StatusEnum.All
  reviewRating: number = 0;
  reviewComment: string = '';
  currentOrderId: string = '';

  selectedOrderDetails:OrderDetailDTO={
  orderNumber:0,
  orderDate:'',
  status:0,
  items:{$id:'',$values:[]},
  restaurantName: '',
  restaurantLocation: '',
  restaurantPhone: '',
  delivaryName: '',
  orderTimeToComplete: '', // TimeSpan -> ISO string or duration format
  customerAddress: '',
  delivaryPhone:'',
  subTotal: 0,
  delivaryPrice: 0,
  totalPrice: 0
}  
ErrorMessageDetails='';
IsLoadingDetails=true
selectedOrderId:number=0
private overlayRef?: OverlayRef;
isDetailsModalOpen=false;
isReviewModalOpen=false;
@ViewChild('DetailsModal') DetailsModal!: TemplateRef<any>;
@ViewChild('ReviewModal') ReviewModal!: TemplateRef<any>;
@ViewChild('printSection') printSection!: ElementRef;
// @ViewChild('printHeader') printHeader!: ElementRef;


  
  OrdersView:OrderViewDTO[]=[]
  ErrorMessage:string=''
  successMessage:string=''
  constructor(
    private orderservice:CustomerService,
      private overlay: Overlay,
  private vcr: ViewContainerRef,
  private reviewService: ReviewService,

  )
  {}
  async ngOnInit(){
    await this.getOrders();
  }
getOrders() {
  this.orderservice.getallOrders().subscribe({
    next: (res) => {
      this.OrdersView = res.$values||[];
      console.log("orders fetched successfully",this.OrdersView)

      if (this.OrdersView.length === 0) {
        this.ErrorMessage = 'No orders found.';
      } else {
        this.ErrorMessage = '';
      }
    },
    error: (err) => {
      console.error('Error fetching Orders:', err);
      this.ErrorMessage = 'Failed to load Orders. Please try again later.';
      this.successMessage=""

    }
  });
}
getitems(items:string[]){
  var itemsnames:string=""
  items.forEach(element => {
    itemsnames+=(element+', ')
  });
  return itemsnames
}
   openModal(templateName: 'details' | 'review',orderId:string) {
    debugger;
        const modalTemplate = templateName === 'details' ? this.DetailsModal : this.ReviewModal;

if (!modalTemplate) {
      console.warn(`${templateName} not ready yet`);
      return;
    }

    if (!this.overlayRef) {
      this.overlayRef = this.overlay.create({
        hasBackdrop: true,
        backdropClass: 'modal',
        panelClass: 'modal-container',
        scrollStrategy: this.overlay.scrollStrategies.block()
      });

      this.overlayRef.backdropClick().subscribe(() => this.closeModal());
    }

    if (!this.overlayRef.hasAttached()) {
      const portal = new TemplatePortal(modalTemplate, this.vcr);
      this.overlayRef.attach(portal);
    }
    if(templateName=='details')
      {this.getOrderDetails(orderId)}
    else if (templateName === 'review') {
    this.currentOrderId = orderId;   // ✅ Save current order id
    this.reviewRating = 0;
    this.reviewComment = '';
  }
  }
  closeModal() {
    if (this.overlayRef) {
      this.overlayRef.dispose();
      this.overlayRef = undefined;
    }
    this.IsLoadingDetails=true
  }
  getOrderDetails(orderId:string){
      this.orderservice.getorderdetails(orderId) .subscribe({
    next: (res) => {
      this.IsLoadingDetails=false
      this.selectedOrderDetails = res||null;
      console.log("order fetched successfully",this.selectedOrderDetails)

    },
    error: (err) => {
      this.IsLoadingDetails=false

      console.error('Error fetching Order :', err);
      this.ErrorMessageDetails = 'Failed to load Order. Please try again later.';

    }
  })
    
  }
   getStatusIcon(status: StatusEnum): string {
    const iconMap = {
      [StatusEnum.All]:'fa-warn',
      [StatusEnum.WaitingToConfirm]: 'fa-clock',
      [StatusEnum.Preparing]: 'fa-utensils',
      [StatusEnum.Out_for_Delivery]: 'fa-bell',
      [StatusEnum.Delivered]: 'fa-truck',
      [StatusEnum.Cancelled]: 'fa-ban'
    };
    return iconMap[status] || 'fa-info-circle';
  }

  getStatusClass(status: StatusEnum): string {
    const classMap = {
      [StatusEnum.All]:'',
      [StatusEnum.WaitingToConfirm]: 'status-waiting',
      [StatusEnum.Preparing]: 'status-preparing',
      [StatusEnum.Out_for_Delivery]: 'status-delivery',
      [StatusEnum.Delivered]: 'status-delivered',
      [StatusEnum.Cancelled]:'Cancelled'
    };
    return classMap[status] || 'status-pending';
  }
  formatTime(time: string): string {
  const parts = time.split(':'); // ["01", "20", "36.7000000"]
  return `${parts[0]}:${parts[1]}`; // "01:20"
}


//filter orders by status
FilterOrders(status:StatusEnum){
  debugger
 this.SelectStatus=status
  this.orderservice.getorderforcustomerbystatus(status).subscribe({
    next: (res) => {
      this.OrdersView = res.$values||[];
      console.log(`orders for status ${StatusEnum [status]} fetched successfully`,this.OrdersView)

    },
    error: (err) => {
      console.error(`Error fetching Orders for status ${StatusEnum[status]}:`, err);
      this.ErrorMessage = 'Failed to load Orders. Please try again later.';
      this.successMessage=""

    }
  });
}

 printOrder() {
    const printContents = this.printSection.nativeElement.innerHTML;
    // const printHeader = this.printHeader.nativeElement.innerHTML;

    const iframe = document.createElement('iframe');

  iframe.style.position = 'absolute';
  iframe.style.width = '0';
  iframe.style.height = '0';
  iframe.style.border = '0';
  document.body.appendChild(iframe);

  const doc = iframe.contentWindow?.document;
  if (doc) {
    doc.open();
    doc.write(`
      <html>
        <head>
          <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
          <link rel="stylesheet" href="http://localhost:4200/src/app/components/CustomerDashboard/customer-orders/customer-orders.css">
        </head>
        <body>${printContents}</body>
      </html>
    `);
    doc.close();

    iframe.contentWindow?.focus();
    iframe.contentWindow?.print();
  }

  // remove iframe after printing
  setTimeout(() => document.body.removeChild(iframe), 1000);  // printWindow.print();
  }

submitReview() {
  const customerId = sessionStorage.getItem('userId');   // ✅ Get from session
  console.log('Rating:', this.reviewRating);
  console.log('Comment:', this.reviewComment);
  // هنا تحط الكود اللي يبعت الـ review للـ API

  const review: ReviewDTO = {
    customerId: customerId ?? '', // fallback to empty string if null
    orderId: this.currentOrderId,
   
    //restaurantId: this.selectedOrderDetails?.restaurantId || '', // لو عندك
    rating: this.reviewRating,
    comment: this.reviewComment
  };

  this.reviewService.createReview(review).subscribe({
    next: () => {
      this.successMessage = 'Review submitted successfully!';
      this.closeModal();
    },
    error: (err) => {
      console.error('Error submitting review:', err);
      this.ErrorMessage = 'Failed to submit review. Please try again later.';
    }
  });
}

}
export enum ViewStatusEnum {
  Waiting = 1,
  Preparing = 2,
  'In Route' = 3,
  Delivered = 4,
  Cancelled = 5
}