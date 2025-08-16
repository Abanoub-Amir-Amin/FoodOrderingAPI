import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-customer-header',
  imports: [CommonModule],
  templateUrl: './customer-header.html',
  styleUrl: './customer-header.css'
})
export class CustomerHeader{
  @Input() collapsed = false;
  @Input() screenWidth = 0;
    getHeadClass(): string {
    let styleClass = '';
    if (this.collapsed && this.screenWidth > 768) {
      styleClass = 'head-trimmed';
    } else {
      styleClass = 'head-md-screen';
    }
    return styleClass;
  }

}
