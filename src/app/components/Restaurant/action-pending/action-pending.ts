import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-action-pending',
  templateUrl: './action-pending.html',
  styleUrls: ['./action-pending.css']
})
export class ActionPendingComponent {
  constructor(private router: Router) {}

  goHome() {
    this.router.navigate(['/home']);
  }
}
