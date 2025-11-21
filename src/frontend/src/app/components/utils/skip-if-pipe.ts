import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'skipIf',
  pure: true // Pure pipe for performance (only recalculates when inputs change)
})
export class SkipIfPipe implements PipeTransform {
  
  transform<T>(
    items: T[] | null | undefined,
    predicate: ((item: T) => boolean) | keyof T,
    value?: any
  ): T[] {
    if (!Array.isArray(items) || items.length === 0) {
      return [];
    }
    
    if (typeof predicate === 'function') {
      return items.filter(item => !predicate(item));
    }
    
    return items.filter(item => item[predicate] !== value);
  }
}