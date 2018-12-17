import { Injectable } from '@angular/core';

@Injectable()
export class SearchService {
  search(s: string, values: string[]): boolean {
    console.log('search', 's', s);
    console.log('search', 'values', values);
    values.forEach(v => console.log('search', 'values', 'v', v, typeof v));
    return values.some(v => v.toLocaleLowerCase().includes(s));
  }
}
