import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { MessageService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  private isBrowser!: boolean;
  constructor(private messageService: MessageService,
    @Inject(PLATFORM_ID) platformid:object
  ) {
    this.isBrowser==isPlatformBrowser(platformid);
  }

  showSuccess(detail: string, summary = 'Success',key="top-right") {
    // if(this.isBrowser){
    this.messageService.add({
      key:key,
      severity: 'success',
      summary,
      detail,
    });
  // }
  }
  
  showError(detail: string, summary = 'Error',key="top-right") {
  //  if(this.isBrowser){
    this.messageService.add({
      key:key,
      severity: 'error',
      summary,
      detail,
    });
  // }
  }
  showInfo(detail: string, summary = 'Info',key="top-right") {
   if(this.isBrowser){
    this.messageService.add({
      key:key,
      severity: 'info',
      summary,
      detail,
    });
    }
  }
  showWarn(detail: string, summary = 'Warning',key="top-right") {
    if(this.isBrowser){
    this.messageService.add({
      key:key,
     severity: 'warning',
      summary,
      detail,
    });
    }
  }
   showCustom(severity: string, summary: string, detail: string, life = 3000,key="top-right") {
   if(this.isBrowser){
    this.messageService.add({
      key:key,
      severity,
      summary,
      detail,
      life
    });
  }
  }

  clear() {
    this.messageService.clear();
  }
}
