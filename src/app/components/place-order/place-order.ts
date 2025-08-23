import { CommonModule, NgClass } from '@angular/common';
import { Component } from '@angular/core';
import { ShoppingCart } from '../../services/shoppingCart/shopping-cart';
import { ActivatedRoute } from '@angular/router';
// import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-place-order',
  imports: [CommonModule],
  templateUrl: './place-order.html',
  styleUrl: './place-order.css'
})
export class PlaceOrder {
  isLoading=true;
  private sessionId=''
  constructor(
  private cartservices:ShoppingCart,
  private route: ActivatedRoute
  ){}
  isOrderSuccess:boolean=false;
  async ngOnInit(): Promise<void> {
  // debugger;
    try{
     this.route.queryParams.subscribe(params => {
     this.sessionId= params['session_id'];
     });
    console.log("Stripe Session ID:", this.sessionId); this.isOrderSuccess= await this.PlaceOrder(this.sessionId);
      console.log(this.isOrderSuccess);
      this.isLoading=false
    }
    catch(err){
      console.log("error in place order"+err);
    }

}
async TryAgain(){
  this.isLoading=true
  try{
     this.route.queryParams.subscribe(params => {
     this.sessionId= params['session_id'];
     });
    console.log("Stripe Session ID:", this.sessionId);

      this.isOrderSuccess= await this.PlaceOrder(this.sessionId);
      console.log(this.isOrderSuccess);

    }
    catch(err){
      console.log("error in place order"+err);
    }
}
 PlaceOrder(sessionId:string):Promise<boolean>{
  return new Promise((resolve,reject) =>{
    this.cartservices.placeOrder(sessionId).subscribe({
      next:(res) =>{
        console.log(res);
        this.isLoading=false
        resolve(true);
      },
      error:(err) => {
        this.isLoading=false
        console.log(err);
     reject(false);
      }
    });

  });    
 }

}
