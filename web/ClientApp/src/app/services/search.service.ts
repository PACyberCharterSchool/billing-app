import { Injectable } from '@angular/core';

@Injectable()
export class SearchService {
  search(s: string, values: string[]): boolean {
    return values.some(v => v.toLocaleLowerCase().includes(s.toLocaleLowerCase()));
  }
}
