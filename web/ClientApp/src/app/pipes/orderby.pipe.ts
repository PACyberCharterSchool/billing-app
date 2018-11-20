import { Pipe, PipeTransform } from '@angular/core';

class OrderByArgs {
  property: string | string[];
  direction: 1 | -1;
}

@Pipe({
  name: 'orderBy'
})
export class OrderByPipe implements PipeTransform {
  private arrayifyProperties(props: string | string[]): string[] {
    switch (typeof props) {
      case 'string':
        return [<string>props];
      case 'object':
        return <string[]>props;
    }
  }

  private uniqueFilter(value: string, index: number, self: string[]): boolean {
    return self.indexOf(value) === index;
  }

  transform(items: Array<any>, args: OrderByArgs): any {
    // TODO(Erik): when would this happen?
    if (!items) {
      return;
    }

    if (!args.property) {
      return items;
    }

    const props = this.arrayifyProperties(args.property).filter(this.uniqueFilter);
    const dir = args.direction;

    return items.sort((a, b) => {
      // TODO(Erik): can this ever happen?
      if (!a || !b) {
        return;
      }

      let val = 0;
      props.forEach(prop => {
        if (val !== 0) {
          return;
        }

        const l = prop === 'schoolDistrict' ? a[prop].name.toUpperCase() : a[prop];
        const r = prop === 'schoolDistrict' ? b[prop].name.toUpperCase() : b[prop];

        if (l < r) {
          val = 1;
        } else if (l > r) {
          val = -1;
        } else {
          val = 0;
        }
      });

      return val * dir;
    });
  }
}
