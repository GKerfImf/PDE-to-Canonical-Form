x, y = var('x y')
_C = var('_C')
Y = function('Y')(x)

forget()

Test = 1
if Test == 0:
    dxx = x; dxy = 2*x; dyy = (x-1)
    dx = 0; dy = 0
    d = 0
    eqForX = x < 0
    eqForY = Y(x) == Y(x)
elif Test == 1:
    dxx = Y(x)^2; dxy = 2*x*Y(x); dyy = -3*x^2
    dx = -Y(x)^2/x; dy = 3*x^2/Y(x)
    d = 0
    eqForX = x > 0
    eqForY = Y(x) > 0

m = matrix(2, 2, [dxx,dxy/2, dxy/2,dyy])
det = m.det().simplify_full()
show("det", m.subs(Y(x) == y), " = ", det.subs(Y(x) == y))

print 'Характеристическое уравнение: '
cEq = dxx * diff(Y,x)^ 2 - dxy * diff(Y,x) + dyy == 0; show(cEq.subs(Y(x) == y))

print 'Решили диффур: '
ODEs = solve(cEq, diff(Y,x)); show(ODEs)

dlh0 = solve( [det < 0,  eqForX, eqForY ], x,y)
deq0 = solve( [det == 0, eqForX, eqForY ], x,y)
dgt0 = solve( [det > 0,  eqForX, eqForY ], x,y)

if len(dlh0) > 0:
    print 'det < 0'
    show('det < 0,', dlh0)
    #assume(dlh0)

    f1(x,y,_C) = desolve(ODEs[0], Y).subs(Y(x) == y)
    f2(x,y,_C) = desolve(ODEs[1], Y).subs(Y(x) == y)
    show("y1 = ",f1, "; y2 = ", f2)

    try:
        u = solve(y == f1, _C)[0]; u = u.rhs()
        v = solve(y == f2, _C)[0]; v = v.rhs()
    except:
        u = solve(f1, _C)[0]; u = u.rhs()
        v = solve(f2, _C)[0]; v = v.rhs()

elif len(deq0) > 0:
    print 'det == 0'
    show('det == 0,', deq0)
    assume(deq0)

    f1(x,y,_C) = desolve(ODEs[0], Y)
    f2(x,y,_C) = Y(x)                                # <= тут можно указать свою ф-ю, если det == 0
    show("y1 = ",f1, "; y2 = ", f2)

    try: u = solve(Y(x) == f1, _C)[0]; u = u.rhs().subs(Y(x) == y)
    except: u = solve(f1, _C)[0]; u = u.rhs().subs(Y(x) == y)
    v = f2; v = v.subs(Y(x) == y)

elif len(dgt0) > 0:
    print 'det:'
    show('det > 0,', dgt0)
    assume(dgt0)
    f1(x,y,_C) = desolve(ODEs[0], Y)
    f2(x,y,_C) = desolve(ODEs[1], Y)
    show("y1 = ",f1, "; y2 = ", f2)

    try:
        t1 = solve(Y(x) == f1, _C)[0]; t1 = t1.rhs().subs(Y(x) == y)
        t2 = solve(Y(x) == f2, _C)[0]; t2 = t2.rhs().subs(Y(x) == y)
    except:
        t1 = solve(f1, _C)[0]; t1 = t1.rhs()
        t2 = solve(f2, _C)[0]; t2 = t2.rhs()

    u = (t1 + t2).real() / 2
    v = (t1 - t2).imag() / 2


print 'Получили такую замену:'
show(u.simplify_full())
show(v.simplify_full())

print 'Якобиан замены:'
matrix(2,2, [diff(u,x), diff(u,y), diff(v,x), diff(v,y)]).det().simplify_full().show()

try:
    U = function('U')(u,v)
except:
    U = function('U')(u(x,y),v(x,y))

print 'Всякая фигня:'
Ux = U.diff(x); show("U_x = ", Ux.simplify_full())
Uy = U.diff(y); show("U_y = ", Uy.simplify_full())
Uxx = Ux.diff(x); show("U_xx = ", Uxx.simplify_full())
Uxy = Ux.diff(y); show("U_xy = ", Uxy.simplify_full())
Uyy = Uy.diff(y); show("U_yy = ", Uyy.simplify_full())

print 'Канонический вид:'
t = (dxx*Uxx + dxy*Uxy + dyy*Uyy + dx*Ux + dy*Uy + d*U).subs(Y == y) == 0;
t = t.simplify_full().expand(); t.show()
︡