import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardSummaryDto {
  deliveredOrders: number;
  inProcessOrders: number;
  cancelledOrders: number;
}

@Injectable({
  providedIn: 'root',
})
export class DashboardSummaryService {
  private apiUrl = 'http://localhost:5000/api/order/';

  constructor(private http: HttpClient) {}

  getSummary(restaurantId: string): Observable<DashboardSummaryDto> {
    // match my backend url as /api/order/{restaurantId}/dashboard-summary
    const url = `${this.apiUrl}${restaurantId}/dashboard-summary`;
    return this.http.get<DashboardSummaryDto>(url);
  }
}
