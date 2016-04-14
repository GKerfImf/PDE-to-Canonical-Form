
type expr =
    | C of double 
    | Var of int
    | Plus of expr * expr
    | Minus of expr * expr
    | Mult of expr * expr
    | Subs of expr * expr
    // TODO: ^, sqrt, cos, sin, exp, log, neg
with override this.ToString() = 
        match this with
        | Var v -> match v with 0 -> "x" | 1 -> "y" | 2 -> "z" | _ -> "w"
        | C c -> c.ToString()
        | Plus(e1,e2) -> "(" + e1.ToString() + " + " + e2.ToString() + ")"  
        | Minus(e1,e2) -> "(" + e1.ToString() + " - " + e2.ToString() + ")"  
        | Mult(e1,e2) -> "(" + e1.ToString() + " * " + e2.ToString() + ")"  
        | Subs(e1,e2) -> "(" + e1.ToString() + " / " + e2.ToString() + ")"  

let random = new System.Random()      //??    


let rec eval_help_optsubs e x subst = 
    match e with
    | Var 0 -> if subst then C x else Var 0 
    | Var v -> if subst then Var (v - 1) else Var v
    | C c -> C c
    | Plus(e1,e2) ->
        let e1', e2' = eval_help_optsubs e1 x subst, eval_help_optsubs e2 x subst
        match e1', e2' with 
        |C c1, C c2 -> C (c1 + c2) 
        | e1, e2 when e1' = e2' -> Mult(C 2.0, e1')
        | _ -> Plus(e1',e2') 
    | Minus(e1,e2) ->
        let e1', e2' = eval_help_optsubs e1 x subst, eval_help_optsubs e2 x subst
        match e1', e2' with 
        |C c1, C c2 -> C (c1 - c2)
        | e1, e2 when e1 = e2 -> C 0.0 
        | _ -> Minus(e1',e2')     
    | Mult(e1,e2) ->
        let e1', e2' = eval_help_optsubs e1 x subst, eval_help_optsubs e2 x subst
        match e1', e2' with C c1, C c2 -> C (c1 * c2) | _ -> Mult(e1',e2') 
    | Subs(e1,e2) ->
        let e1', e2' = eval_help_optsubs e1 x subst, eval_help_optsubs e2 x subst
        match e1', e2' with 
        |C c1, C c2 -> C (c1 / c2)
        | e1', e2' when e1' = e2' -> C 1.0
        | _ -> Subs(e1',e2')

let eval0 e = eval_help_optsubs e 0.0 false  // ~~> simplify?
let eval1 e x = eval_help_optsubs e x true
let eval2 e x y = eval1 (eval1 e x) y
let eval3 e x y z = eval1 (eval2 e x y) z


//????
let rec splitTree e f =
    let b = (new System.Random()).Next(0,4)
    match e with
    | Plus(e1',e2') when b = 0 -> (fun e -> Plus(f e, e2')), e1'
    | Plus(e1',e2') when b = 1 -> (fun e -> Plus(e1', f e)), e2'
    | Plus(e1',e2') when b = 2 -> 
        let newf, e' = splitTree e1' f 
        (fun x -> Plus(newf x, e2')), e'
    | Plus(e1',e2') when b = 3 ->
        let newf, e' = splitTree e2' f 
        (fun x -> Plus(e1', newf x)), e'

    | Mult(e1',e2') when b = 0 -> (fun e -> Mult(f e, e2')), e1'
    | Mult(e1',e2') when b = 1 -> (fun e -> Mult(e1', f e)), e2'
    | Mult(e1',e2') when b = 2 -> 
        let newf, e' = splitTree e1' f 
        (fun x -> Mult(newf x, e2')), e'
    | Mult(e1',e2') when b = 3 ->
        let newf, e' = splitTree e2' f 
        (fun x -> Mult(e1', newf x)), e'

    | Minus(e1',e2') when b = 0 -> (fun e -> Minus(f e, e2')), e1'
    | Minus(e1',e2') when b = 1 -> (fun e -> Minus(e1', f e)), e2'
    | Minus(e1',e2') when b = 2 -> 
        let newf, e' = splitTree e1' f 
        (fun x -> Minus(newf x, e2')), e'
    | Minus(e1',e2') when b = 3 ->
        let newf, e' = splitTree e2' f 
        (fun x -> Minus(e1', newf x)), e'

    | Subs(e1',e2') when b = 0 -> (fun e -> Subs(f e, e2')), e1'
    | Subs(e1',e2') when b = 1 -> (fun e -> Subs(e1', f e)), e2'
    | Subs(e1',e2') when b = 2 -> 
        let newf, e' = splitTree e1' f 
        (fun x -> Subs(newf x, e2')), e'
    | Subs(e1',e2') when b = 3 ->
        let newf, e' = splitTree e2' f 
        (fun x -> Subs(e1', newf x)), e'

    | x -> f, x

let сrossover e1 e2 =
    let fc1, ec1 = splitTree e1 (fun e -> e)
    let fc2, ec2 = splitTree e2 (fun e -> e)
    fc1 ec2 |> eval0  //, fc2 ec1



let rec get_rand_tree h =
    let get_rand_tree_0 =
        let v = 0.5
        if (random.NextDouble()) > v then random.Next(0,9) |> float |> C else Var 0

    if h <= 0
        then 
            get_rand_tree_0
        else
            let t = random.Next(0, 4) 
            match t with 
            | 0 -> Plus(get_rand_tree (h - 1), get_rand_tree (h - 1)) 
            | 1 -> Minus(get_rand_tree (h - 1), get_rand_tree (h - 1))
            | 2 -> Mult(get_rand_tree (h - 1), get_rand_tree (h - 1))
            //| 3 -> Subs(get_rand_tree (h - 1), get_rand_tree (h - 1))
            | _ -> get_rand_tree_0

//TODO: More mutations
let mutation e =
    //let shortTree e =
    //    let rec helper e h = 
    //        match e with
    //        | Plus(e1,e2) when h > 0 -> Plus(helper e1 (h-1), helper e2 (h-1))
    //        | Minus(e1,e2) when h > 0 ->  Minus(helper e1 (h-1), helper e2 (h-1))
    //        | Mult(e1,e2) when h > 0 -> Mult(helper e1 (h-1), helper e2 (h-1)) 
    //        | Subs(e1,e2) when h > 0 -> Subs(helper e1 (h-1), helper e2 (h-1))
    //        | _ -> Var 0  
    //    helper e 4

    let fc, c = splitTree e (fun x -> x)
    5 |> get_rand_tree |> fc |> eval0
      

let fitness f e = 
    let arg = [1.0; 2.0; 3.0; 16.0]
    let l1 = List.map f arg
    let l2 = List.map (eval1 e) arg |> List.map (fun e -> match e with C c -> c | _ -> failwith "kek")

    List.fold2 (fun acc x y -> acc + pown (x-y) 2) 0.0 l1 l2


[<EntryPoint>]
let main argv =

    let f x = x*x*x + x + 2.0

    let N = 20
    let K = 5

    let mutable r = (List.replicate N (Var 0)) |> List.map mutation

    while fitness f r.Head > 5.0 do                        
        let r2 = r |> List.sortBy (fun e -> fitness f e)

        let r3 = r2.[0..K-1] @ ([for i in 0 .. (N-1) do 
                                    for j in 0 .. (N-1) 
                                        -> сrossover r2.[i] r2.[j] ])

        let r4 = r3 |> List.sortBy (fun e -> fitness f e)
        let r5 = r4.[0..K-1] @ r4.[K..N-1] |> List.map mutation
        let r6 = r5 |> List.sortBy (fun e -> fitness f e)

        r <- r6
        r.Head.ToString() |> printfn "%A"
        fitness f r.Head |> printfn "%A"
         
    r.Head.ToString() |> printfn "%A"

    0
