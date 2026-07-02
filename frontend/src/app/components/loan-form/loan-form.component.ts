import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { LoanService } from '../../services/loan.service';

@Component({
  selector: 'app-loan-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
  ],
  templateUrl: './loan-form.component.html',
  styleUrls: ['./loan-form.component.scss'],
})
export class LoanFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly loanService = inject(LoanService);
  private readonly snackBar = inject(MatSnackBar);
  private readonly router = inject(Router);

  isSubmitting = false;

  readonly form = this.fb.group({
    applicantName: ['', [Validators.required, Validators.minLength(2)]],
    contractId: ['', [Validators.required, Validators.minLength(3)]],
    taxId: ['', [Validators.required, Validators.minLength(11)]],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
  });

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { applicantName, contractId, taxId, amount } = this.form.getRawValue();
    this.isSubmitting = true;

    this.loanService
      .createLoan({
        applicantName: applicantName!,
        contractId: contractId!,
        taxId: taxId!,
        amount: amount!,
      })
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.snackBar.open('Loan application created successfully.', 'Close', {
            duration: 5000,
          });
          this.router.navigate(['/loans']);
        },
        error: () => {
          this.isSubmitting = false;
          this.snackBar.open('Unable to create loan application. Please try again.', 'Close', {
            duration: 8000,
          });
        },
      });
  }

  cancel(): void {
    this.router.navigate(['/loans']);
  }
}
