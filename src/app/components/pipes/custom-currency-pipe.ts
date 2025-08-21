import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'customCurrency'
})
export class CustomCurrencyPipe implements PipeTransform {

  transform(
    value: number | string | null | undefined,
    currencySymbol: string = 'EGP',  // العملة الافتراضية EGP
    decimalLength: number = 2,
    symbolFirst: boolean = false     // لو true يحط الرمز قبل الرقم
  ): string {
    if (value == null || value === '') return '';

    // تحويل القيمة لرقم
    let numericValue = Number(value);
    if (isNaN(numericValue)) return String(value);

    // صياغة الرقم بفواصل الألوف
    const formatted = numericValue.toFixed(decimalLength)
      .replace(/\B(?=(\d{3})+(?!\d))/g, ',');

    // اختيار مكان الرمز
    return symbolFirst ? `${currencySymbol} ${formatted}` : `${formatted} ${currencySymbol}`;
  }

}
