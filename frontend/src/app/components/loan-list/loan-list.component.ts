import { Component, OnInit } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpErrorResponse } from '@angular/common/http';
import { Loan } from '../../models/loan.model';
import { LoanService } from '../../services/loan.service';

@Component({
  selector: 'app-loan-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CurrencyPipe,
    MatTableModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './loan-list.component.html',
  styleUrls: ['./loan-list.component.scss'],
})
export class LoanListComponent implements OnInit {
  displayedColumns: string[] = [
    'applicantName',
    'amount',
    'currentBalance',
    'status',
    'actions',
  ];

  loans: Loan[] = [];
  isLoading = false;
  loadError = '';
  paymentAmounts: Record<number, number | null> = {};
  paymentErrors: Record<number, string> = {};
  payingLoanIds = new Set<number>();

  constructor(private readonly loanService: LoanService) {}

  ngOnInit(): void {
    this.loadLoans();
  }

  loadLoans(): void {
    this.isLoading = true;
    this.loadError = '';

    this.loanService.getLoans().subscribe({
      next: (loans) => {
        this.loans = loans;
        this.isLoading = false;
      },
      error: () => {
        this.loadError = 'Unable to load loans. Please check if the API is running.';
        this.isLoading = false;
      },
    });
  }

  pay(loan: Loan): void {
    const amount = this.paymentAmounts[loan.id];

    if (!amount || amount <= 0) {
      this.paymentErrors[loan.id] = 'Enter a payment amount greater than zero.';
      return;
    }

    this.paymentErrors[loan.id] = '';
    this.payingLoanIds.add(loan.id);

    this.loanService.registerPayment(loan.id, amount).subscribe({
      next: () => {
        this.payingLoanIds.delete(loan.id);
        this.paymentAmounts[loan.id] = null;
        this.loadLoans();
      },
      error: (error: HttpErrorResponse) => {
        this.payingLoanIds.delete(loan.id);
        this.paymentErrors[loan.id] =
          error.error?.message ?? 'Payment failed. Please try again.';
      },
    });
  }

  isPaying(loanId: number): boolean {
    return this.payingLoanIds.has(loanId);
  }

  isPaid(loan: Loan): boolean {
    return loan.status === 'paid';
  }
}
