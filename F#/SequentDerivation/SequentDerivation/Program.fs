open Expr

// TODO: --> Test
//48а              
let e1_1 = Or(Not (Var A),Var B)
let e1_2 = Or(Var C, Not(Var B))
let e1_3 = Imp(Var A, Var C)
let e1_4 = Imp(Var A, Not(Var C))
let s1 = Seq ([e1_1;e1_2],[e1_3;e1_4])

//48б
let e2_1 = Imp(Var A, Var B)
let e2_2 = Imp(Var B, Var C)
let e2_3 = Imp(Var A, Var C)
let s2 = Seq ([e2_1;e2_2],[e2_3])

//48в
let e3_1 = Or(Not(Var A),Var B)
let e3_2 = Or(Var C, Not(Var B))
let e3_3 = Imp(Var A, Var C)
let e3_4 = Imp(Var A, Not(Var C))
let s3 = Seq ([e3_1;e3_2],[e3_3;e3_4])

//48г
let e4_1 = Imp(Or(Var A, Var B), And(Var A, Var C))
let s4 = Seq ([],[e3_1])

//48ж
let e5_1 = Not (And(Var A, Or(Var B, Var C))) 
let e5_2 = Or(And(Var A, Var B), Var C)
let s5 = Seq ([e5_1],[e5_2])

//48л
let e6_1 = Imp(Var A,Imp(Var B, Var B))
let e6_2 = Var B
let s6 = Seq ([e6_1],[e6_2])

//48у
let e7_1 = Eq(Var A, Var B)
let e7_2 = Eq(Imp(Var C, Var A),Imp(Var C, Var B))
let s7 = Seq ([e7_1],[e7_2])

//48ф
let e8_1 = Imp(Var A,Imp(Var B, Var B))
let e8_2 = Var A
let s8 = Seq ([e8_1],[e8_2])

let e9_1 = Imp(Var A,Imp(Var B, Var B))
let e9_2 = And(Not (Var A), Or (Var C, Var C) )
let e9_3 = Not (Imp (Var B, Not(Var C)))
let e9_4 = Or(Not (Var B), Var C)
let s9 = Seq ([e9_1;e9_2],[e9_3;e9_4])

//49 
let s10 = Seq ([Eq(Var A, Var B)],[Eq(Var B, Var A)])

//50
let s11 = Seq ([],[Eq( And(Var A, Or (Var B, Var C)), Or(And(Var A, Var B), And(Var A, Var C)))])


//let temp = Seq ([Forall(A,Exists(B, Imp(Var A, Var B)))],[Forall(A, Imp(Var A, Var A))]) 

[<EntryPoint>]
let main argv =
    SeqCalc.run s11 |> ignore
    //QuantifierSeqCalc.run temp |> ignore 
    0