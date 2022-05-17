10 input "Enter fibonacci sequence number";n
20 gosub 1000
30 print "The fibonacci number at position";n;"is";a(n)
40 end

1000 a(0) = 0 : a(1) = 1 : a(2) = 1
1010 if n <= 2 then return
1020 n = n - 1
1030 gosub 1010
1040 n = n + 1
1050 a(n) = a(n-1) + a(n-2)
1060 rem runprint "#1060 n=";n;"a(";n;")=";a(n);"a(";n-1;")=";a(n-1);"a(";n-2;")=";a(n-2)
1070 return
