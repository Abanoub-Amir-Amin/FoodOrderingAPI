import { CommonModule, NgClass } from '@angular/common';
import { Component } from '@angular/core';
import { ShoppingCart } from '../../services/shoppingCart/shopping-cart';
// import { NgModel } from '@angular/forms';

@Component({
  selector: 'app-place-order',
  imports: [CommonModule],
  templateUrl: './place-order.html',
  styleUrl: './place-order.css'
})
export class PlaceOrder {
  isLoading=true;
  constructor(
  private cartservices:ShoppingCart
  ){}
  isOrderSuccess:boolean=false;
  async ngOnInit(): Promise<void> {
  // debugger;
    try{
      this.isOrderSuccess= await this.PlaceOrder();
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
      this.isOrderSuccess= await this.PlaceOrder();
      console.log(this.isOrderSuccess);

    }
    catch(err){
      console.log("error in place order"+err);
    }
}
 PlaceOrder():Promise<boolean>{
  return new Promise((resolve,reject) =>{
    this.cartservices.placeOrder().subscribe({
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
