module Tests

open Xunit
open Glicko2.Calculator


let factory = PlayerFactory(1500.0, 0.06)
let calculator = Calculator(0.5, 1500.0, 0.06)

[<Fact>]
let ``µ is correct to 4dp - paper v0 case``() =
    let v0  = factory.makePlayer(1400.0, 30.0, 0.06)
    Assert.Equal(round (v0.µ, 4),-0.5756)

[<Fact>]
let ``g(x) is correct to 4dp - paper v2 case``() =
    let hero  = factory.makePlayer(1500.0, 200.0, 0.06)
    let v1  = factory.makePlayer(1550.0, 100.0, 0.06)
    Assert.Equal(round (g 1.7269, 4), 0.7242)

// E(µ, µj, φj)
[<Fact>]
let ``E(µ, µj, φj) is correct to 3dp - paper v1 case``() =
    let hero  = factory.makePlayer(1500.0, 200.0, 0.06)
    let v1  = factory.makePlayer(1550.0, 100.0, 0.06)
    Assert.Equal(round (e hero.µ v1.µ v1.φ, 3), 0.432)
    
[<Fact>]
let ``Rating at the end of run matches paper rating``() = 
    let hero  = factory.makePlayer(1500.0, 200.0, 0.06)
    let v0  = factory.makePlayer(1400.0, 30.0, 0.06)
    let v1  = factory.makePlayer(1550.0, 100.0, 0.06)
    let v2  = factory.makePlayer(1700.0, 300.0, 0.06)

    let game1 = Game(hero, v0, Result.Win)
    let game2 = Game(hero, v1, Result.Loss)
    let game3 = Game(hero, v2, Result.Loss)

    let games = [game1;game2;game3]

    let run = RatingsRun(games)

    let result = calculator.update (run, hero)

    // rounding errors mean the result on the paper is actually slightly imprecise
    Assert.InRange(result.r, 1464.050, 1464.07)

[<Fact>]
let ``Vol to 6 d.p. at the end of run matches paper rating``() = 
    let hero  = factory.makePlayer(1500.0, 200.0, 0.06)
    let v0  = factory.makePlayer(1400.0, 30.0, 0.06)
    let v1  = factory.makePlayer(1550.0, 100.0, 0.06)
    let v2  = factory.makePlayer(1700.0, 300.0, 0.06)

    let game1 = Game(hero, v0, Result.Win)
    let game2 = Game(hero, v1, Result.Loss)
    let game3 = Game(hero, v2, Result.Loss)

    let games = [game1;game2;game3]

    let run = RatingsRun(games)

    let result = calculator.update (run, hero)
    Assert.Equal(round (result.σ, 6), 0.059996)
