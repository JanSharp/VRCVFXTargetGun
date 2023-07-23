
# Syncing

bytes synced by effects full sync with x amount of active effects

10: 456
9: 424
8: 384
7: 352
6: 312
5: 280

calculated diffs showing how many bytes each individual effect added, extended calculation past the tested values down to 0

10-9: 32
9-8: 40
8-7: 32
7-6: 40
6-5: 32
5-4: 40
4-3: 32
3-2: 40
2-1: 32
1-0: 40

don't ask me why but this is what we get
it should be 44 for every effect if there was zero overhead
but instead we get less?
are they actually doing compression as they should be?
if yes, how is it still so large after that?
also if yes, how is it consistent? I mean sure I've only tested 5 values, but they have a pattern.
It's just weird, but let's just take the average:
36 per effect

late joiner sync byte count for effects:
96 per gun with any active effects + 36 per effect

so you can now have about 657.7777777777778 active effects spread across 5 guns for it to get as bad as it was in `0.2.0` with 12 active effects (although the fact that it was 12 doesn't really matter, it was all of the accumulated overhead of arrays, really).
however in reality I'd say 100 to 200 active effects is a much more practical limit than 650.



This test came before the one above however there were very few changes affecting the amount of bytes being synced between the 2. I don't remember exactly if I changed anything in between them unfortunately but I'm fairly certain because I wouldn't setup the entire test setup just to test something I'm going to change anyway.

raw values (total bytes synced by both EffectDescriptor and effect full sync (which is a different script between the 2 tests) since instance launch, which means it doesn't reset between joins):

`0.2.0` first join: 25224
`0.2.0` second join: 49384

`0.2.1` first join: 1112
`0.2.1` second join: 1568


the first join is bigger because everything that requested serialization ends up syncing twice. This won't happen in the real world because there are pretty much always at least 2 people in the map already.
the second join is what matters

test with 6, 3, 1 active effects (so 10 total effects spread out across 3 effects)

`0.2.0`:
(first join: 25224)
second join: 24160

`0.2.1`:
(first join: 1112)
second join: 456

gigantic improvement
