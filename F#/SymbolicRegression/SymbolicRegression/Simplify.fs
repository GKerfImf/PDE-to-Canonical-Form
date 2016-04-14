module Simplify
//
//let rec compl e =
//    match e with
//    | Plus(e1,e2) | Minus(e1,e2) | Mult(e1,e2) | Subs(e1,e2) -> 1 + compl e1 + compl e2
//    | C x -> 0
//    | Var x -> 1
//
////let rec left_assoc e = 
////    match e with                                   
////    | Plus(e1, Plus(e2,e3)) -> Plus(Plus(e1,e2),e3)
////    | Mult(e1, Mult(e2,e3)) -> Mult(Mult(e1,e2),e3)
////    | x -> x
//
//let rec commut e = 
//    let rec mult_hepl e = 
//        match e with                                   
//        | Plus(e1, e2) when compl e1 < compl e2 -> Plus(e2, e1)
//        | Mult(e1, e2) when compl e1 < compl e2 -> Mult(e2, e1)
//                           
//        | Plus(e1,e2) -> Plus(mult_hepl e1, mult_hepl e2)
//        | Minus(e1,e2) -> Minus(mult_hepl e1, mult_hepl e2)
//        | Mult(e1,e2) -> Mult(mult_hepl e1, mult_hepl e2)
//        | Subs(e1,e2) -> Subs(mult_hepl e1, mult_hepl e2)
//        | C x -> C x
//        | Var x -> Var x
//    let e' = mult_hepl e 
//    if e = e' then e' else commut e'
//     
//let rec var_up e = 
//    let rec mult_hepl e = 
//        match e with                                   
//        | Plus(Plus(e1,e2), e3) when compl e2 < compl e3 -> Plus(Plus(e1,e3), e2)
//        | Minus(Minus(e1,e2), e3) when compl e2 < compl e3 -> Minus(Minus(e1,e3), e2)
//        | Mult(Mult(e1,e2), e3) when compl e2 < compl e3 -> Mult(Mult(e1,e3), e2)
//        | Subs(Subs(e1,e2), e3) when compl e2 < compl e3 -> Subs(Subs(e1,e3), e2)
//
//        | Minus(Plus(e1,e2), e3) when compl e2 < compl e3 -> Plus(Minus(e1,e3), e2)
//        | Plus(Minus(e1,e2), e3) when compl e2 < compl e3 -> Minus(Plus(e1,e3), e2)
//
//        | Plus(e1,e2) -> Plus(mult_hepl e1, mult_hepl e2)
//        | Minus(e1,e2) -> Minus(mult_hepl e1, mult_hepl e2)
//        | Mult(e1,e2) -> Mult(mult_hepl e1, mult_hepl e2)
//        | Subs(e1,e2) -> Subs(mult_hepl e1, mult_hepl e2)
//        | C x -> C x
//        | Var x -> Var x
//    let e' = mult_hepl e 
//    if e = e' then e' else var_up e' 
//
//let rec mult_0__mult_1 e = 
//    let rec mult_hepl e = 
//        match e with                                   
//        | Mult(e1, C 0.0) -> C 0.0
//        | Mult(C 0.0, e2) -> C 0.0
//        | Mult(e1, C 1.0) -> e1
//        | Mult(C 1.0, e2) -> e2 
//                           
//        | Plus(e1,e2) -> Plus(mult_hepl e1, mult_hepl e2)
//        | Minus(e1,e2) -> Minus(mult_hepl e1, mult_hepl e2)
//        | Mult(e1,e2) -> Mult(mult_hepl e1, mult_hepl e2)
//        | Subs(e1,e2) -> Subs(mult_hepl e1, mult_hepl e2)
//        | C x -> C x
//        | Var x -> Var x
//    let e' = mult_hepl e 
//    if e = e' then e' else mult_0__mult_1 e' 
//
//let rec plus_minus_0 e = 
//    let rec plus_hepl e = 
//        match e with                                   
//        | Plus(e1, C 0.0) -> e1
//        | Plus(C 0.0, e2) -> e2
//        | Minus(e1, C 0.0) -> e1
//                           
//        | Plus(e1,e2) -> Plus(plus_hepl e1, plus_hepl e2)
//        | Minus(e1,e2) -> Minus(plus_hepl e1, plus_hepl e2)
//        | Mult(e1,e2) -> Mult(plus_hepl e1, plus_hepl e2)
//        | Subs(e1,e2) -> Subs(plus_hepl e1, plus_hepl e2)
//        | C x -> C x
//        | Var x -> Var x
//    let e' = plus_hepl e 
//    if e = e' then e' else plus_minus_0 e' 
//
//
//let rec distr e = 
//    let rec distr_hepl e = 
//        match e with                                   
//        | Mult(e1, Plus(e2, e3)) -> Plus(Mult(e1,e2), Mult(e1, e3))
//        | Mult(e1, Minus(e2, e3)) -> Minus(Mult(e1,e2), Mult(e1, e3))
//        | Mult(Plus(e1, e2), e3) -> Plus(Mult(e1,e3), Mult(e2, e3))
//        | Mult(Minus(e1, e2), e3) -> Minus(Mult(e1,e3), Mult(e2, e3))
//                           
//        | Plus(e1,e2) -> Plus(distr_hepl e1, distr_hepl e2)
//        | Minus(e1,e2) -> Minus(distr_hepl e1, distr_hepl e2)
//        | Mult(e1,e2) -> Mult(distr_hepl e1, distr_hepl e2)
//        | Subs(e1,e2) -> Subs(distr_hepl e1, distr_hepl e2)
//        | C x -> C x
//        | Var x -> Var x
//    let e' = distr_hepl e 
//    if e = e' then e' else distr e' 
//
