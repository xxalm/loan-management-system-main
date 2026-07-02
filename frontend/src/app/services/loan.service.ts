import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Loan } from '../models/loan.model';

export interface CreateLoanPayload {
  applicantName: string;
  amount: number;
  contractId: string;
  taxId: string;
}

@Injectable({
  providedIn: 'root',
})
export class LoanService {
  private readonly apiUrl = `${environment.apiBaseUrl}/api/loans`;

  constructor(private readonly http: HttpClient) {}

  getLoans(): Observable<Loan[]> {
    return this.http.get<Loan[]>(this.apiUrl);
  }

  getLoanById(id: number): Observable<Loan> {
    return this.http.get<Loan>(`${this.apiUrl}/${id}`);
  }

  registerPayment(id: number, amount: number): Observable<Loan> {
    return this.http.post<Loan>(`${this.apiUrl}/${id}/payment`, { amount });
  }

  createLoan(payload: CreateLoanPayload): Observable<Loan> {
    return this.http.post<Loan>(this.apiUrl, payload);
  }
}
