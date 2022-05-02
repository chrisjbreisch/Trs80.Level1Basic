10 rem * sample answer 23-2 *
100 cls
110 p.:p.
120 p."Enter the number of one of the following investments"
130 p.
140 p."   1 - Certificate of Deposit"
150 p."   2 - Bank Savings Account"
160 p."   3 - Credit Union"
170 p."   4 - Mortgage Loan"
180 p.:in. " Investment";f
190 on f goto 1000,2000,3000,4000
200 goto 100: rem used if number not between 1 and 4
1000 rem * certificate of deposit program goes here *
1010 p."The C.D. program has yet to be written."
1020 gos.10000:g.100
2000 rem * bank savings account *
2010 cls:p.:p."This routine calculates simple interest on"
2020 p."dollars held in deposit for a specified period"
2030 p."using a specified percentage of interest.":p.
2040 p.:in."How large is the deposit (in dollars)";p
2050 in."How long will you leavfe it in (in days)";d
2060 in."What interest rate to you expect (in %)";r
2070 cls:p.:p.:p."For a starting principal of $";p;" at a"
2080 p."rate of ";r;" % for ";d;" days, the interest "
2090 p."amounts to $";
2100 rem interest = (% / yr) / (days/yr) * days * principal
2200 i = r/100 / 365 * d * p
2300 p.:p."    ", " $";i
2400 end
3000 rem * credit union program goes here *
3010 p."The C.U. program has yet to be written."
3020 gos.10000:g.100
4000 rem * mortgage loan program goes here *
4010 p."The M.L. program has yet to be written."
4020 gos.10000:g.100
10000 f.i=1to8000000:n.i:ret.