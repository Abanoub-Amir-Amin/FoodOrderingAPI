import { Component } from '@angular/core';





import { MainLayoutComponent } from "../../../layout/main-layout/main-layout.component";
import { FooterComponent } from "../../../layout/footer/footer.component";
import { SliderComponent } from "../slider/slider.component";

@Component({
  selector: 'app-mainuser',
  imports: [ MainLayoutComponent, FooterComponent, SliderComponent],
  templateUrl: './mainuser.component.html',
  styleUrl: './mainuser.component.css'
})
export class MainuserComponent {

}
