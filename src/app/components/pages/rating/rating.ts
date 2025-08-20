import { C } from '@angular/cdk/keycodes';
import { CommonModule } from '@angular/common';
import { Component, Input, Output , EventEmitter } from '@angular/core';

@Component({
  selector: 'app-rating',
  imports: [CommonModule],
  templateUrl: './rating.html',
  styleUrl: './rating.css'
})
export class Rating {

@Input() maxStars = 5;        // عدد النجوم
  @Input() rating = 0;          // التقييم الحالي
  @Input() readOnly = false;    // لو true يمنع التغيير
  @Output() ratingChange = new EventEmitter<number>();

  setRating(star: number) {
    if (!this.readOnly) {
      this.rating = star;
      this.ratingChange.emit(this.rating);
    }
  }
}