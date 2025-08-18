import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'statustitle'
})
export class StatustitlePipe implements PipeTransform {

 transform(value: any, enumObj: any): string {
    if (value === null || value === undefined) {
      return '';
    }

    const key = enumObj[value]; // نفس اللى بتعمليه {{StatusEnum[value]}}

    if (!key) return '';

    // TitleCase
    return key.charAt(0).toUpperCase() + key.slice(1).toLowerCase();
  }


}
