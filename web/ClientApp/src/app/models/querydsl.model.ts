type BooleanOperator = 'eq' | 'ne' | 'gt' | 'ge' | 'lt' | 'le' | 'has' | 'bgn' | 'end';

export class BooleanClause {
  constructor(
    private identifier: string,
    private operator: BooleanOperator,
    private value: any,
  ) { }

  stringify(): string {
    return `(${this.identifier} ${this.operator} ${this.value})`;
  }
}

export function has(identifier: string, value: string): BooleanClause {
  return new BooleanClause(identifier, 'has', value);
}

type CompoundOperator = 'and' | 'or';

export class CompoundClause {
  constructor(
    private left: Clause,
    private operator: CompoundOperator,
    private right: Clause,
  ) { }

  stringify(): string {
    return `(${this.left.stringify()} ${this.operator} ${this.right.stringify()})`;
  }
}

export function or(left: Clause, right: Clause): CompoundClause {
  return new CompoundClause(left, 'or', right);
}

export type Clause = BooleanClause | CompoundClause;
