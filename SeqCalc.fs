open System

type X = A | B | C
    with override this.ToString() = match this with A -> "A" | B -> "B" | C -> "C"

type expr =
    | Val of X 
    | Not of expr
    | And of expr * expr
    | Or of expr * expr
    | Imp of expr * expr
    | Eq of expr * expr
with override this.ToString() = 
        match this with
        | Val a -> a.ToString()
        | Not expr -> match expr with Val a -> "-" + expr.ToString() | _ -> "-(" + expr.ToString() + ")"  
        | And (exp1,exp2) ->
            match exp1 with
            | Val _ | Not _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " /\ " +
            match exp2 with 
            | Val _ | Not _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"
        | Or (exp1,exp2) ->
            match exp1 with
            | Val _ | Not _ | And _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " \/ " +
            match exp2 with 
            | Val _ | Not _ | And _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"
        | Imp (exp1,exp2) ->
            match exp1 with
            | Val _ | Not _ | And _ | Or _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " => " +
            match exp2 with 
            | Val _ | Not _ | And _ | Or _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"
        | Eq (exp1,exp2) ->
            match exp1 with
            | Val _ | Not _ | And _ | Or _ | Imp _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " <=> " +
            match exp2 with 
            | Val _ | Not _ | And _ | Or _ | Imp _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"


type seq = Seq of (expr list) * (expr list)
with override this.ToString() =
    match this with 
    Seq (a,b) ->
        (a |> List.map (fun x -> x.ToString()) |> String.concat "; ") + (if (a.IsEmpty) then "_" else "")
        + "  -->  " +
        (b |> List.map (fun x -> x.ToString()) |> String.concat "; ") + (if (b.IsEmpty) then "_" else "")

type derivation = seq list
let derToString a = (a |> List.map (fun x -> x.ToString()) |> String.concat "\n")

    
type List<'T> with
    static member splitAt3 ind lst = 
        let l,r = lst |> List.splitAt ind
        l, r.Head, r.Tail

let rArNot (der: derivation) = 
    match der.Head with
    Seq (a,b) -> 
        [Seq(a @ (b |> List.filter (fun x -> match x with Not (_) -> true | _ -> false) |> List.map (fun x -> match x with Not b -> b)), 
                List.filter (fun x -> match x with Not _ -> false | _ -> true) b)] @ der.Tail

let lArNot (der: derivation) = 
    match der.Head with
    Seq (a,b) -> 
        [Seq( List.filter (fun x -> match x with Not _ -> false | _ -> true) a,
            b @ (a |> List.filter (fun x -> match x with Not _ -> true | _ -> false) |> List.map (fun x -> match x with Not b -> b)))] @ der.Tail


let rArAnd (der: derivation) = 
    match der.Head with
    Seq (a,b) ->
        try
            let l, c, r = b |> List.splitAt3 (b |> List.findIndex (fun x -> match x with And(_,_) -> true | _ -> false))
            [Seq (a, l @ [c |> (fun x -> match x with And(e1,e2) -> e1)] @ r );
             Seq (a, l @ [c |> (fun x -> match x with And(e1,e2) -> e2)] @ r )] @ der.Tail
        with
            |_ -> der

let lArAnd (der: derivation) = 
    match der.Head with
    Seq (a,b) -> 
        [Seq(a |> List.collect (fun x -> match x with And(e1,e2) -> [e1;e2] | e -> [e]), b)] @ der.Tail


let rArOr (der: derivation) = 
    match der.Head with
    Seq (a,b) -> 
        [Seq(a, List.collect (fun x -> match x with Or(e1,e2) -> [e1;e2] | e -> [e]) b)] @ der.Tail

let lArOr (der: derivation) = 
    match der.Head with
    Seq (a,b) ->
        try
            let l,c,r = a |> List.splitAt3 (a |> List.findIndex (fun x -> match x with Or(_,_) -> true | _ -> false))
            [Seq (l @ [c |> (fun x -> match x with Or(e1,e2) -> e1)] @ r, b);
             Seq (l @ [c |> (fun x -> match x with Or(e1,e2) -> e2)] @ r, b)] @ der.Tail
        with
            |_ -> der


let rArImp (der: derivation) = 
    match der.Head with
    Seq (a,b) -> 
        [Seq(a @ (b |> List.filter (fun x -> match x with Imp(_,_) -> true | _ -> false) |> List.map (fun x -> match x with Imp(e1,e2) -> e1)), 
                List.map (fun x -> match x with Imp(e1,e2) -> e2 | e -> e) b)] @ der.Tail

let lArImp (der: derivation) = 
    match der.Head with
    Seq (a,b) ->
        try
            let l,c,r = a |> List.splitAt3 (a |> List.findIndex (fun x -> match x with Imp(_,_) -> true | _ -> false))
            [Seq (l @ [c |> (fun x -> match x with Imp(e1,e2) -> e2)] @ r, b);
             Seq (l @ r, [c |> (fun x -> match x with Imp(e1,e2) -> e1)] @ b)] @ der.Tail
        with
            |_ -> der


let rArEq (der: derivation) = 
    match der.Head with
    Seq (a,b) ->
        try
            let l,c,r = b |> List.splitAt3 (b |> List.findIndex (fun x -> match x with Eq(_,_) -> true | _ -> false))
            [Seq (a @ [c |> (fun x -> match x with Eq(e1,e2) -> e1)],r @ [c |> (fun x -> match x with Eq(e1,e2) -> e2)]);
             Seq (a @ [c |> (fun x -> match x with Eq(e1,e2) -> e2)],r @ [c |> (fun x -> match x with Eq(e1,e2) -> e1)])] @ der.Tail
        with
            |_ -> der

let lArEq (der: derivation) = 
    match der.Head with
    Seq (a,b) ->
        try
            let l,c,r = a |> List.splitAt3 (a |> List.findIndex (fun x -> match x with Eq(_,_) -> true | _ -> false))
            [Seq (l @ (c |> (fun x -> match x with Eq(e1,e2) -> [e1;e2])) @ r, b);
             Seq (l@r, b @ (c |> (fun x -> match x with Eq(e1,e2) -> [e1;e2])))] @ der.Tail
        with
            |_ -> der



let axiom (der: derivation) = 
    match der.Head with
    |Seq ([],_) -> der
    |Seq (_,[]) -> der
    |Seq (a,b) ->
        let b = a |> List.map (fun x -> (List.exists (fun y -> x = y) b)) |> List.reduce (||)
        if b then der.Tail else der


 //48а              
let e1_1 = Or(Not (Val A),Val B)
let e1_2 = Or(Val C, Not(Val B))
let e1_3 = Imp(Val A, Val C)
let e1_4 = Imp(Val A, Not(Val C))
let s1 = Seq ([e1_1;e1_2],[e1_3;e1_4])

//48б
let e2_1 = Imp(Val A, Val B)
let e2_2 = Imp(Val B, Val C)
let e2_3 = Imp(Val A, Val C)
let s2 = Seq ([e2_1;e2_2],[e2_3])

//48в
let e3_1 = Or(Not(Val A),Val B)
let e3_2 = Or(Val C, Not(Val B))
let e3_3 = Imp(Val A, Val C)
let e3_4 = Imp(Val A, Not(Val C))
let s3 = Seq ([e3_1;e3_2],[e3_3;e3_4])

//48г
let e4_1 = Imp(Or(Val A, Val B), And(Val A, Val C))
let s4 = Seq ([],[e3_1])

//48ж
let e5_1 = Not (And(Val A, Or(Val B, Val C))) 
let e5_2 = Or(And(Val A, Val B), Val C)
let s5 = Seq ([e5_1],[e5_2])

//48л
let e6_1 = Imp(Val A,Imp(Val B, Val B))
let e6_2 = Val B
let s6 = Seq ([e6_1],[e6_2])

//48у
let e7_1 = Eq(Val A, Val B)
let e7_2 = Eq(Imp(Val C, Val A),Imp(Val C, Val B))
let s7 = Seq ([e7_1],[e7_2])

//48ф
let e8_1 = Imp(Val A,Imp(Val B, Val B))
let e8_2 = Val A
let s8 = Seq ([e8_1],[e8_2])

let e9_1 = Imp(Val A,Imp(Val B, Val B))
let e9_2 = And(Not (Val A), Or (Val C, Val C) )
let e9_3 = Not (Imp (Val B, Not(Val C)))
let e9_4 = Or(Not (Val B), Val C)
let s9 = Seq ([e9_1;e9_2],[e9_3;e9_4])

//49 
let s10 = Seq ([Eq(Val A, Val B)],[Eq(Val B, Val A)])

//50
let s11 = Seq ([],[Eq( And(Val A, Or (Val B, Val C)), Or(And(Val A, Val B), And(Val A, Val C)))])

[<EntryPoint>]
let main argv =
    let mutable cur_der = [s11]
    let mutable bool_for_smile = true
    while cur_der.IsEmpty |> not do
        printfn "\n%A"  <| (cur_der |> derToString)
        let temp = cur_der
        
        match true with
        | _ when cur_der <> (temp |> axiom) -> cur_der <- temp |> axiom; printf "axiom\n"
        | _ when cur_der <> (temp |> rArNot) -> cur_der <- temp |> rArNot; printf "rArNot\n"
        | _ when cur_der <> (temp |> lArNot) -> cur_der <- temp |> lArNot; printf "lArNot\n"
        | _ when cur_der <> (temp |> rArImp) -> cur_der <- temp |> rArImp; printf "rArImp\n"
        | _ when cur_der <> (temp |> rArOr) -> cur_der <- temp |> rArOr; printf "rArOr\n"
        | _ when cur_der <> (temp |> lArAnd) -> cur_der <- temp |> lArAnd; printf "lArAnd\n"
        | _ when cur_der <> (temp |> rArAnd) -> cur_der <- temp |> rArAnd; printf "rArAnd\n"  
        | _ when cur_der <> (temp |> lArOr) -> cur_der <- temp |> lArOr; printf "lArOr\n"
        | _ when cur_der <> (temp |> lArImp) -> cur_der <- temp |> lArImp; printf "lArImp\n"
        | _ when cur_der <> (temp |> rArEq) -> cur_der <- temp |> rArEq; printf "rArEq\n"
        | _ when cur_der <> (temp |> lArEq) -> cur_der <- temp |> lArEq; printf "lArEq\n"
        | _ -> cur_der <- []; bool_for_smile <- false

    if bool_for_smile then printfn "\n^-^\n" else printf "\n:(\n"
        
    0                                  