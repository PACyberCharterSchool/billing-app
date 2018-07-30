import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';

enum BUTTON_IDS {
  PaymentsBtn = 'payments-btn',
  RefundsBtn = 'refunds-btn'
}

@Component({
  selector: 'app-payments-refunds-home',
  templateUrl: './payments-refunds-home.component.html',
  styleUrls: ['./payments-refunds-home.component.scss']
})
export class PaymentsRefundsHomeComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  handleClick(event) {
    const target = event.currentTarget;
    const idAttr = target.attributes.id;
    const id = idAttr.value;

    if (id) {
      this.handlePaymentsRefundsHomeBtnSelection(id);
    }
  }

  handlePaymentsRefundsHomeBtnSelection(id: string) {
    switch (id) {
      case BUTTON_IDS.PaymentsBtn:
        this.router.navigate(['/payments-refunds', { outlets: { 'action': ['payments'] } }]);
        break;
      case BUTTON_IDS.RefundsBtn:
        this.router.navigate(['/payments-refunds', { outlets: { 'action': ['refunds'] } }]);
        break;
      default:
        this.router.navigate(['/payments-refunds', { outlets: { 'action': ['home'] } }]);
        break;
    }
  }

}
