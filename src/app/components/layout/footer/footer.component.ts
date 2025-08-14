import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  imports: [],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.css'
})
export class FooterComponent {
 email = '';

  subscribe() {
    if (this.email) {
      // Here you’d implement a call to your backend API
      console.log(`Subscribing: ${this.email}`);
    }
    }
}
