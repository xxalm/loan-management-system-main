import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError, EMPTY } from 'rxjs';
import { Loan, LoanStatus } from '../../models/loan.model';
import { LoanService } from '../../services/loan.service';

type StatusFilter = 'all' | LoanStatus;

@Component({
  selector: 'app-loan-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CurrencyPipe,
    RouterLink,
    MatTableModule,
    MatPaginatorModule,
    MatSelectModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './loan-list.component.html',
  styleUrls: ['./loan-list.component.scss'],
})
export class LoanListComponent implements OnInit {
  @ViewChild(MatPaginator)
  set matPaginator(paginator: MatPaginator) {
    if (paginator) {
      this.dataSource.paginator = paginator;
    }
  }

  displayedColumns: string[] = [
    'contractId',
    'applicantName',
    'taxId',
    'amount',
    'currentBalance',
    'status',
    'details',
    'actions',
  ];

  dataSource = new MatTableDataSource<Loan>([]);
  loans: Loan[] = [];
  isLoading = false;
  loadError = '';
  paymentAmounts: Record<number, number | null> = {};
  payingLoanIds = new Set<number>();

  filterName = '';
  filterTaxId = '';
  filterContractId = '';
  filterStatus: StatusFilter = 'all';

  private readonly snackBarDurationMs = 10000;

  constructor(
    private readonly loanService: LoanService,
    private readonly snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadLoans();
  }

  applyFilters(): void {
    const nameFilter = this.filterName.trim().toLowerCase();
    const taxIdFilter = this.filterTaxId.trim().toLowerCase();
    const contractFilter = this.filterContractId.trim().toLowerCase();

    this.dataSource.data = this.loans.filter((loan) => {
      const matchesName =
        !nameFilter || loan.applicantName.toLowerCase().includes(nameFilter);

      const matchesTaxId =
        !taxIdFilter || loan.taxId.toLowerCase().includes(taxIdFilter);

      const matchesContractId =
        !contractFilter || loan.contractId.toLowerCase().includes(contractFilter);

      const matchesStatus =
        this.filterStatus === 'all' || loan.status === this.filterStatus;

      return matchesName && matchesTaxId && matchesContractId && matchesStatus;
    });

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  loadLoans(): void {
    this.isLoading = true;
    this.loadError = '';

    this.loanService.getLoans().subscribe({
      next: (loans) => {
        this.loans = loans;
        this.applyFilters();
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
      this.showError('Enter a payment amount greater than zero.');
      return;
    }

    this.payingLoanIds.add(loan.id);

    this.loanService
      .registerPayment(loan.id, amount)
      .pipe(
        catchError((error: HttpErrorResponse) => {
          const message =
            error.error?.message ?? 'Payment failed. Please try again.';
          this.showError(message);
          this.payingLoanIds.delete(loan.id);
          return EMPTY;
        })
      )
      .subscribe({
        next: () => {
          this.payingLoanIds.delete(loan.id);
          this.paymentAmounts[loan.id] = null;
          this.loadLoans();
        },
      });
  }

  clearFilters(): void {
    this.filterName = '';
    this.filterTaxId = '';
    this.filterContractId = '';
    this.filterStatus = 'all';
    this.applyFilters();
  }

  isPaying(loanId: number): boolean {
    return this.payingLoanIds.has(loanId);
  }

  isPaid(loan: Loan): boolean {
    return loan.status === 'paid';
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', {
      duration: this.snackBarDurationMs,
    });
  }
}
