module Expr

type X = A | B | C
    with override this.ToString() = match this with A -> "A" | B -> "B" | C -> "C"

type expr =
    | Var of X 
    | Not of expr
    | And of expr * expr
    | Or of expr * expr
    | Imp of expr * expr
    | Eq of expr * expr
with override this.ToString() = 
        match this with
        | Var a -> a.ToString()
        | Not expr -> match expr with Var a -> "-" + expr.ToString() | _ -> "-(" + expr.ToString() + ")"  
        | And (exp1,exp2) ->
            match exp1 with
            | Var _ | Not _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " /\ " +
            match exp2 with 
            | Var _ | Not _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"
        | Or (exp1,exp2) ->
            match exp1 with
            | Var _ | Not _ | And _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " \/ " +
            match exp2 with 
            | Var _ | Not _ | And _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"
        | Imp (exp1,exp2) ->
            match exp1 with
            | Var _ | Not _ | And _ | Or _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " => " +
            match exp2 with 
            | Var _ | Not _ | And _ | Or _ -> exp2.ToString()
            | _ -> "(" + exp2.ToString() + ")"
        | Eq (exp1,exp2) ->
            match exp1 with
            | Var _ | Not _ | And _ | Or _ | Imp _ -> exp1.ToString()
            | _ -> "(" + exp1.ToString() + ")"
            + " <=> " +
            match exp2 with 
            | Var _ | Not _ | And _ | Or _ | Imp _ -> exp2.ToString()
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