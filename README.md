# WarlockTBC
Burning Crusade Warlock damage sim and theorycrafting tool
This project is strictly a personal education project and not intended for public use.
- It has no UI; you change the code and rebuild/rerun to test different scenarios
- It only supports Warlock with no plans to support other classes
- I would not say I am proud of the code, this is a quick and dirty tool to help me answer specific questions I had.

Okay, moving on, here's a bit of what this code can do:

## Single-Fight Sim: Detail View

This view is mostly used for debugging, but is also useful for demonstrating the sim. It simulates battles in a timeline fashion with an event-based internal structure.

![Single-fight damage details](/images/timeline.png)

More useful than this timeline view is the summary view, still for a single fight:

![Single-fight summary](/images/single_fight_summary.png)

You can also view spell stats such as DPCT for a single fight:

![Spell stats](/images/spell_stats.png)

## Multi-Fight Sim: Meta Results

Single fights are sort of interesting, but the real value in the tool comes from the ability to sim the fight thousands of times, to factor out noise from RNG.
By doing this we can take the RNG pretty much out of the picture and get to objective answers that compare the benefits of talent choices, gear choices, and rotation choices.

![Meta Results](/images/meta_results.png)

When doing analysis with meta results, I always look at the Median fight, but it's still useful to know what the worst and best fights look like.

The last main tool in the chest is a helper tool to sim stat scaling to generate stat weights customized for your exact current gear. 
Stat weights definitely depend on your overall gear level, they're not fixed at all. 
That said, the basic conventional wisdom of Hit (up to cap) > Spell Power > Crit/Haste holds true.
At _really low_ gear levels, spell power is actually slightly more valuable than hit, but that doesnt invalidate the conventional wisdom. This can help you determine
exactly when these transitions occur.

![Stat Weights](/images/stat_weights.png)
