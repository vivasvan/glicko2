namespace Glicko2
open System

module Calculator =
    let sq x = x * x
    let inv x = 1./x

    let round (x:float, p: int) = System.Math.Round(x, p)

    let glicko2scale = 173.7178

    let µ r initialRating = (r - initialRating) / glicko2scale 

    let φ rd = rd / glicko2scale 

    let g x = (1.0 / sqrt(1.0 + 3.0 * (sq x/sq Math.PI ))) 

    let e µ µ_j φ_j = (1.0 / (1.0 + exp(-(g φ_j) * (µ - µ_j))) )

    type Result = Win = 1 | Loss = -1 | Draw = 0

    type Player(r: float,  rd: float, σ: float, initialRating: float) = 
       member this.r = r
       member this.rd = rd
       member this.σ = σ
       member this.φ = φ rd
       member this.µ = µ r initialRating

    type PlayerFactory(initialRating: float, initialVol: float) =
        member this.makePlayer (r: float,  rd: float, σ: float) = 
            Player(r,  rd, σ, initialRating)

    type Game(hero: Player, villain: Player, result: Result) = 
        member this.hero = hero
        member this.villain = villain
        member this.result = result

    // NB: The ratings run implementation is hero centric, not agent free! Can optimise 2x.
    type RatingsRun(games: List<Game>) =
        member this.games = games

    let vsummand (gm: Game) = 
        let e_g = e gm.hero.µ gm.villain.µ gm.villain.φ
        sq (g gm.villain.φ) * e_g * (1.0 - e_g)

    let v (run: RatingsRun) = (List.map vsummand run.games) |> List.sum |> inv 
    let score res = match res with
                        | Result.Win -> 1.0
                        | Result.Draw -> 0.5
                        | Result.Loss -> 0.0
                        | _ -> failwithf "Unknown result value %s" (res.ToString())
                                         
    let delta (run: RatingsRun): double =
        let summand (gm: Game): double = g gm.villain.φ * (score gm.result - e gm.hero.µ gm.villain.µ gm.villain.φ)
        (v run) * ((List.map summand run.games) |> List.sum ) 

    type Calculator(tau: float, initialRating: float, initialVol: float) =
        let τ = tau // pick value betw 0.3 - 1.2 , adjust based on predictive accuracy.

        let σ' (run: RatingsRun, player: Player): double =
            let v_0 = v run
            let del = delta run
            let φ_0 = φ player.rd
            let ε = 0.000001
            let a= 2. * log (player.σ)

            let f x =
                (exp x * (sq del - sq φ_0 - v_0 - exp x ) ) / (2.0 * sq (sq φ_0 + v_0 + exp x))  - (x- a)/(sq τ) 

            let findB = 
                let mutable k = 1.0
                while (f (a - k * τ) < 0.0) do
                    k <- k + 1.0

                (a - k * τ)

            let mutable aa = a
            let mutable bb = if sq del > sq φ_0 + v_0 then log (sq del - sq φ_0 - v_0) else findB
            let mutable faa = f aa
            let mutable fbb = f bb

            while (abs (bb - aa) > ε) do
                let c = aa + (aa - bb) * faa / (fbb - faa)          
                if (f c * fbb < 0.0) then
                    aa <- bb 
                    faa <- fbb
                else 
                    faa <- faa / 2.0

                bb <- c
                fbb <- f c 

            exp (aa / 2.0)

        member this.tau = tau
        member this.initialRating = initialRating
        member this.initialVol = initialVol
        member this.update(run: RatingsRun, player: Player)= 
            let σ_1 = σ' (run, player)
            let φ_temp = (sq player.φ  + (sq σ_1)) |> sqrt
            let φ' = 1.0 / sqrt ( 1.0 / sq φ_temp + 1.0/(v run) ) 

            let summand (gm: Game) = 
                let e_g = e gm.hero.µ gm.villain.µ gm.villain.φ
                g gm.villain.φ * (score gm.result - e_g)

            let sum = List.map summand run.games |> List.sum
            let µ' = player.µ + sq φ' * sum

            let rd = φ' * glicko2scale
            let r = µ' * glicko2scale + initialRating
            Player(r,rd, σ_1, initialRating)


