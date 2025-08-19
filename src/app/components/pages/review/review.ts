import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ReviewDTO, ReviewService } from '../../../services/review/review-service';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-review',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './review.html',
  styleUrl: './review.css'
})
export class Review implements OnInit {
 @Input() orderId!: string;
  @Input() restaurantId!: string;
  @Input() customerId!: string;

  @Output() reviewAdded = new EventEmitter<void>(); // عشان نرجع إشعار للـ parent

  reviewForm: FormGroup;

  constructor(private fb: FormBuilder, private review: ReviewService) {
    this.reviewForm = this.fb.group({
      rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
      comment: ['', Validators.required]
    });
  }
  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  submitReview(): void {
    if (this.reviewForm.invalid) return;

    const review: ReviewDTO = {
      customerId: this.customerId,
      orderId: this.orderId,
      restaurantId: this.restaurantId,
      rating: this.reviewForm.value.rating,
      comment: this.reviewForm.value.comment
    };

    this.review.createReview(review).subscribe({
      next: () => {
        alert('Review added successfully!');
        this.reviewForm.reset({ rating: 5, comment: '' });
        this.reviewAdded.emit(); // نرجع للـ parent إنه خلص
      },
      error: (err) => console.error('Error adding review', err)
    });
  }}
