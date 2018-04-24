import { SortableTableDirective } from './sortable-table.directive';
import { SortService } from '../services/sort.service';

describe('SortableTableDirective', () => {
  it('should create an instance', () => {
    const directive = new SortableTableDirective(new SortService());
    expect(directive).toBeTruthy();
  });
});
