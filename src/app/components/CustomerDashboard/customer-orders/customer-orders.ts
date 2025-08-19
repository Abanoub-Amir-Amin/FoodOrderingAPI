import { Component, TemplateRef, ViewChild, ViewContainerRef } from '@angular/core';
import { CustomerService } from '../../../services/customer/customer-service';
import { OrderDetailDTO, OrderItemDto, OrderViewDTO, StatusEnum } from '../../../models/DTO.model';
import { CommonModule } from '@angular/common';
import { Overlay, OverlayModule, OverlayRef } from '@angular/cdk/overlay';
import { PortalModule, TemplatePortal } from '@angular/cdk/portal';
import { StatustitlePipe } from '../../pipes/statustitle-pipe';
@Component({
  selector: 'app-customer-orders',
  imports: [CommonModule,OverlayModule,PortalModule,StatustitlePipe],
  templateUrl: './customer-orders.html',
  styleUrl: './customer-orders.css'
})
export class CustomerOrders {
  StatusEnum=StatusEnum
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
  address: '',
  subTotal: 0,
  delivaryPrice: 0,
  discountAmount: 0,
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


  OrdersView:OrderViewDTO[]=[]
  ErrorMessage:string=''
  successMessage:string=''
  constructor(
    private orderservice:CustomerService,
      private overlay: Overlay,
  private vcr: ViewContainerRef,

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
      this.getOrderDetails(orderId)
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
    };
    return classMap[status] || 'status-pending';
  }
}
