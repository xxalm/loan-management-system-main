using System;
using System.Collections.Generic;
using Fundo.Domain.Entities;
using Fundo.Domain.Enums;

namespace Fundo.Infrastructure.Data.Seed
{
    public static class LoanSeedData
    {
        private static readonly string[] FirstNames =
        {
            "Maria", "João", "Ana", "Carlos", "Bruno", "Fernanda", "Ricardo", "Juliana",
            "Pedro", "Camila", "Lucas", "Patricia", "Rafael", "Mariana", "Gabriel", "Beatriz",
            "Felipe", "Larissa", "Rodrigo", "Amanda", "Diego", "Carolina", "Thiago", "Renata",
            "Gustavo", "Aline", "Marcelo", "Vanessa", "André", "Tatiane", "Eduardo", "Priscila",
            "Leonardo", "Simone", "Henrique", "Daniela", "Vinícius", "Cristina", "Paulo", "Eliane",
            "Roberto", "Sandra", "Antônio", "Márcia", "Fábio", "Luciana", "Sérgio", "Helena",
            "Maurício", "Bianca"
        };

        private static readonly string[] LastNames =
        {
            "Silva", "Santos", "Oliveira", "Souza", "Lima", "Costa", "Ferreira", "Almeida",
            "Pereira", "Rodrigues", "Gomes", "Martins", "Ribeiro", "Carvalho", "Araújo", "Melo",
            "Barbosa", "Nascimento", "Cavalcanti", "Dias", "Castro", "Pinto", "Teixeira", "Moura",
            "Correia", "Cardoso", "Monteiro", "Freitas", "Ramos", "Nunes", "Moreira", "Machado",
            "Farias", "Lopes", "Vieira", "Campos", "Reis", "Mendes", "Nogueira", "Cunha",
            "Batista", "Miranda", "Azevedo", "Tavares", "Borges", "Fonseca", "Aguiar", "Duarte",
            "Macedo", "Peixoto"
        };

        public static Loan[] GetLoans()
        {
            var loans = new List<Loan>(50);
            var random = new Random(42);

            for (var id = 1; id <= 50; id++)
            {
                var amount = Math.Round((decimal)(random.NextDouble() * 24500 + 500), 2);
                var isPaid = id % 2 == 0;
                decimal currentBalance;

                if (isPaid)
                {
                    currentBalance = 0m;
                }
                else
                {
                    var balanceRatio = (decimal)(random.NextDouble() * 0.85 + 0.05);
                    currentBalance = Math.Round(amount * balanceRatio, 2);

                    if (currentBalance <= 0)
                    {
                        currentBalance = amount;
                    }
                }

                loans.Add(new Loan
                {
                    Id = id,
                    Amount = amount,
                    CurrentBalance = currentBalance,
                    ApplicantName = $"{FirstNames[id - 1]} {LastNames[id - 1]}",
                    ContractId = BuildContractId(id),
                    TaxId = BuildTaxId(id),
                    Status = isPaid ? LoanStatus.Paid : LoanStatus.Active
                });
            }

            loans[0] = new Loan
            {
                Id = 1,
                Amount = 1500.00m,
                CurrentBalance = 500.00m,
                ApplicantName = "Maria Silva",
                ContractId = BuildContractId(1),
                TaxId = BuildTaxId(1),
                Status = LoanStatus.Active
            };

            return loans.ToArray();
        }

        private static string BuildContractId(int id) => $"CTR-{id:D5}";

        private static string BuildTaxId(int id)
        {
            var segment = (100 + id) % 1000;
            var suffix = (10 + id) % 100;
            return $"{segment:D3}.{segment:D3}.{segment:D3}-{suffix:D2}";
        }
    }
}
