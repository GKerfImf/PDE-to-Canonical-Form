module SeqCalc

open System
open Expr 

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


let run argv =
    let mutable (cur_der : seq list) = [argv]
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