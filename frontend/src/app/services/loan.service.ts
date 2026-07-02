import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../environments/environment';
import { Loan } from '../models/loan.model';

@Injectable({
  providedIn: 'root',
})
export class LoanService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/loans`;

  constructor(private readonly http: HttpClient) {}

  getLoans(): Observable<Loan[]> {
    return this.http
      .get<Loan[]>(this.apiUrl)
      .pipe(map((loans) => loans.map((loan) => this.enrichLoan(loan))));
  }

  getLoanById(id: number): Observable<Loan> {
    return this.http
      .get<Loan>(`${this.apiUrl}/${id}`)
      .pipe(map((loan) => this.enrichLoan(loan)));
  }

  registerPayment(id: number, amount: number): Observable<Loan> {
    return this.http
      .post<Loan>(`${this.apiUrl}/${id}/payment`, { amount })
      .pipe(map((loan) => this.enrichLoan(loan)));
  }

  private enrichLoan(loan: Loan): Loan {
    return {
      ...loan,
      taxId: loan.taxId ?? this.buildMockTaxId(loan.id),
    };
  }

  private buildMockTaxId(id: number): string {
    const segment = String(100 + id).padStart(3, '0');
    const suffix = String(10 + id).padStart(2, '0');
    return `${segment}.${segment}.${segment}-${suffix}`;
  }
}
