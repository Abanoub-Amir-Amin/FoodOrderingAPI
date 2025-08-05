import { Component } from '@angular/core';

@Component({
  selector: 'app-location',
  imports: [],
  templateUrl: './location.component.html',
  styleUrl: './location.component.css'
})
export class LocationComponent {

  latitude: number | null = null;
  longitude: number | null = null;
  errorMessage: string | null = null;

  getLocation() {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.latitude = position.coords.latitude;
          this.longitude = position.coords.longitude;
        },
        (error) => {
          this.errorMessage = "Unable to retrieve your location.";
        }
      );
    } else {
      this.errorMessage = "Geolocation is not supported by this browser.";
    }
  }
}
