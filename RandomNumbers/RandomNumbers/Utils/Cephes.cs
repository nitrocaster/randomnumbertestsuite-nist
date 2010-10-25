using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomNumbers.Utils {
    class Cephes {

        private const double MACHEP =  1.11022302462515654042E-16;
        private const double MAXLOG =  7.09782712893383996732E2;
        const double LOGPI  =  1.14472988584940017414;
       
        private const double big    =  4.503599627370496e15;
        private const double biginv =  2.22044604925031308085e-16;

        public static double igam(double a, double x) {

            double ans, ax, c, r;

            if (x <= 0 || a <= 0) return 0.0;

            if (x > 1.0 && x > a) return 1.0 - igamc(a, x);

            /* Compute  x**a * exp(-x) / gamma(a)  */
            ax = a * Math.Log(x) - x - lgam(a);
            if (ax < -MAXLOG) return (0.0);

            ax = Math.Exp(ax);

            /* power series */
            r = a;
            c = 1.0;
            ans = 1.0;

            do {
                r += 1.0;
                c *= x / r;
                ans += c;
            }
            while (c / ans > MACHEP);

            return (ans * ax / a);
        }

        public static double igamc(double a, double x) {

            double ans, ax, c, yc, r, t, y, z;
            double pk, pkm1, pkm2, qk, qkm1, qkm2;

            if (x <= 0 || a <= 0) return 1.0;

            if (x < 1.0 || x < a) return 1.0 - igam(a, x);

            ax = a * Math.Log(x) - x - lgam(a);
            if (ax < -MAXLOG) return 0.0;

            ax = Math.Exp(ax);

            /* continued fraction */
            y = 1.0 - a;
            z = x + y + 1.0;
            c = 0.0;
            pkm2 = 1.0;
            qkm2 = x;
            pkm1 = x + 1.0;
            qkm1 = z * x;
            ans = pkm1 / qkm1;

            do {
                c += 1.0;
                y += 1.0;
                z += 2.0;
                yc = y * c;
                pk = pkm1 * z - pkm2 * yc;
                qk = qkm1 * z - qkm2 * yc;
                if (qk != 0) {
                    r = pk / qk;
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                } else
                    t = 1.0;

                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;
                if (Math.Abs(pk) > big) {
                    pkm2 *= biginv;
                    pkm1 *= biginv;
                    qkm2 *= biginv;
                    qkm1 *= biginv;
                }
            } while (t > MACHEP);

            return ans * ax;
        }

        public static double lgam(double x) {

            double p, q, w, z;
 
            double[] A = {
              8.11614167470508450300E-4,
              -5.95061904284301438324E-4,
              7.93650340457716943945E-4,
              -2.77777777730099687205E-3,
              8.33333333333331927722E-2
            };
            double[] B = {
              -1.37825152569120859100E3,
              -3.88016315134637840924E4,
              -3.31612992738871184744E5,
              -1.16237097492762307383E6,
              -1.72173700820839662146E6,
              -8.53555664245765465627E5
            };
            double[] C = {
              /* 1.00000000000000000000E0, */
              -3.51815701436523470549E2,
              -1.70642106651881159223E4,
              -2.20528590553854454839E5,
              -1.13933444367982507207E6,
              -2.53252307177582951285E6,
              -2.01889141433532773231E6
            };
 
            if( x < -34.0 ) {
              q = -x;
              w = lgam(q);
              p = Math.Floor(q);
              if( p == q ) throw new ArithmeticException("lnGamma: Overflow");
              z = q - p;
              if( z > 0.5 ) {
	        p += 1.0;
	        z = p - q;
              }
              z = q * Math.Sin( Math.PI * z );
              if( z == 0.0 ) throw new 
	        ArithmeticException("lnGamma: Overflow");
              z = LOGPI - Math.Log( z ) - w;
              return z;
            }
 
            if( x < 13.0 ) {
              z = 1.0;
              while( x >= 3.0 ) {
	        x -= 1.0;
	        z *= x;
              }
              while( x < 2.0 ) {
	        if( x == 0.0 ) throw new 
	          ArithmeticException("lnGamma: Overflow");
	        z /= x;
	        x += 1.0;
              }
              if( z < 0.0 ) z = -z;
              if( x == 2.0 ) return Math.Log(z);
              x -= 2.0;
              p = x * polevl( x, B, 5 ) / p1evl( x, C, 6);
              return( Math.Log(z) + p );
            }
 
            if( x > 2.556348e305 ) throw new ArithmeticException("lnGamma: Overflow");
 
            q = ( x - 0.5 ) * Math.Log(x) - x + 0.91893853320467274178;
  
            if( x > 1.0e8 ) return( q );
 
            p = 1.0/(x*x);
            if( x >= 1000.0 )
              q += ((   7.9365079365079365079365e-4 * p
		        - 2.7777777777777777777778e-3) *p
	            + 0.0833333333333333333333) / x;
            else
              q += polevl( p, A, 4 ) / x;
            return q;
          }

        public static double p1evl(double x, double[] coef, int N) {

            double ans;
            ans = x + coef[0];

            for (int i = 1; i < N; i++) ans = ans * x + coef[i];

            return ans;
        }

        public static double polevl(double x, double[] coef, int N) {

            double ans;
            ans = coef[0];

            for (int i = 1; i <= N; i++) ans = ans * x + coef[i];

            return ans;
        }

        public static double erf(double x) {
            double y, z;
            double[] T = {
              9.60497373987051638749E0,
              9.00260197203842689217E1,
              2.23200534594684319226E3,
              7.00332514112805075473E3,
              5.55923013010394962768E4
            };
            double[] U = {
              //1.00000000000000000000E0,
              3.35617141647503099647E1,
              5.21357949780152679795E2,
              4.59432382970980127987E3,
              2.26290000613890934246E4,
              4.92673942608635921086E4
            };

            if (Math.Abs(x) > 1.0) return (1.0 - erfc(x));
            z = x * x;
            y = x * polevl(z, T, 4) / p1evl(z, U, 5);
            return y;
        }

        public static double erfc(double a) {
            double x, y, z, p, q;

            double[] P = {
              2.46196981473530512524E-10,
              5.64189564831068821977E-1,
              7.46321056442269912687E0,
              4.86371970985681366614E1,
              1.96520832956077098242E2,
              5.26445194995477358631E2,
              9.34528527171957607540E2,
              1.02755188689515710272E3,
              5.57535335369399327526E2
            };
            double[] Q = {
              //1.0
              1.32281951154744992508E1,
              8.67072140885989742329E1,
              3.54937778887819891062E2,
              9.75708501743205489753E2,
              1.82390916687909736289E3,
              2.24633760818710981792E3,
              1.65666309194161350182E3,
              5.57535340817727675546E2
            };

            double[] R = {
              5.64189583547755073984E-1,
              1.27536670759978104416E0,
              5.01905042251180477414E0,
              6.16021097993053585195E0,
              7.40974269950448939160E0,
              2.97886665372100240670E0
            };
            double[] S = {
              //1.00000000000000000000E0, 
              2.26052863220117276590E0,
              9.39603524938001434673E0,
              1.20489539808096656605E1,
              1.70814450747565897222E1,
              9.60896809063285878198E0,
              3.36907645100081516050E0
            };

            if (a < 0.0) x = -a;
            else x = a;

            if (x < 1.0) return 1.0 - erf(a);

            z = -a * a;

            if (z < -MAXLOG) {
                if (a < 0) return (2.0);
                else return (0.0);
            }

            z = Math.Exp(z);

            if (x < 8.0) {
                p = polevl(x, P, 8);
                q = p1evl(x, Q, 8);
            } else {
                p = polevl(x, R, 5);
                q = p1evl(x, S, 6);
            }

            y = (z * p) / q;

            if (a < 0) y = 2.0 - y;

            if (y == 0.0) {
                if (a < 0) return 2.0;
                else return (0.0);
            }
            return y;
        }

        public static double normal(double x) {
            double arg, result, sqrt2 = 1.414213562373095048801688724209698078569672;

            if (x > 0) {
                arg = x / sqrt2;
                result = 0.5 * (1 + erf(arg));
            } else {
                arg = -x / sqrt2;
                result = 0.5 * (1 - erf(arg));
            }

            return (result);
        }
    
    }
}
