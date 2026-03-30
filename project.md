# Iron Strata

---

## TABLE OF CONTENTS

1. The Vision & Philosophy
2. Narrative Foundation
3. World-Building & Lore
4. Core Game Loop
5. The Three Pillars — Deep Dive
6. The Strata — Mechanical Architecture
7. Wagon System — Complete Catalog
8. Passenger System — The Human Component
9. Pilot System — Characters & Meta-Progression
10. Enemy Factions — The Silicon Life
11. Combat & Defense Systems
12. The Economy of Desperation
13. Procedural Generation — The Megastructure
14. Node System & World Map
15. Boss Encounters — The Gates
16. Progression & Unlocks
17. UI/UX Philosophy & Interface Design
18. Audio Design
19. Visual Design & Aesthetic Direction
20. Technical Specification
21. Development Roadmap
22. Market Positioning & Business Strategy
23. Appendices

---

---

# PART ONE — VISION & FOUNDATIONS

---

## 1. THE VISION & PHILOSOPHY

### 1.1 The Elevator Pitch

Iron Strata is a high-stakes survival roguelike set within a boundless, brutalist world of infinite concrete and forgotten machinery. Players command The Strata — a moving fortress that serves as the last functioning ecosystem for the remnants of humanity. Every run is a desperate journey through procedurally generated tunnels, hostile silicon-based lifeforms, and the crushing indifference of an architecture that has long outlived its purpose.

The game sits at the intersection of three proven genres: the spatial strategy of a deckbuilder, the tension of a tower-defense, and the emotional depth of a colony management sim. What binds them is a single, unyielding question the player must answer in every session: _How far can you go before it all falls apart?_

### 1.2 The Design Philosophy — "Industrial Existentialism"

The term "Industrial Existentialism" is not merely an aesthetic label. It is the design lens through which every system in Iron Strata is built and evaluated.

**Industrial** refers to the game's relationship with systems, machines, and processes. Everything in this game is a process. The train is a process. The passengers' lives are processes. Even survival is a process — a series of inputs and outputs that can be optimized, but never perfected. The player is not a hero. They are an engineer trying to keep a machine from seizing up.

**Existentialism** refers to the weight placed on individual meaning within an indifferent universe. The Megastructure does not care about humanity. It does not care about The Strata. It simply exists — vast, cold, and eternal. The act of surviving within it is itself the meaning. There is no guaranteed salvation at the end of the run. There is only the journey, the choices made, and the people lost along the way.

Together, these two words produce the game's emotional core: _the beauty and tragedy of keeping things alive in a world designed to kill them._

### 1.3 Foundational Design Tenets

Every design decision in Iron Strata should be evaluated against these four tenets:

**Tenet I — Everything Has Weight.** No action should feel weightless. Placing a wagon is a commitment. Assigning a passenger is a relationship. Firing a weapon costs something. Death is permanent and personal. The player must always feel that their choices matter because they genuinely do.

**Tenet II — Complexity Emerges, It Is Not Imposed.** The base systems of the game must be learnable within minutes. The depth comes from the interaction between systems, not from the complexity of any individual system. A new player should be able to place a gun wagon and understand what it does. A veteran player should discover, 50 hours in, that pairing that gun wagon with a specific passenger trait and a particular tunnel curvature creates an emergent strategy no one anticipated.

**Tenet III — The World Is the Antagonist.** The Silicon Life is not the villain. The Megastructure itself is. It is the pressure, the scale, the darkness, the weight of infinite concrete above and below. Enemy encounters are expressions of that pressure, not the source of it. The game should always feel like the environment is the thing trying to kill you, and the enemies are simply its instruments.

**Tenet IV — Loss Is a Teacher, Not a Punishment.** Death ends a run. That is permanent and non-negotiable. But every run leaves something behind — a fragment of knowledge, a revealed map segment, a blueprint, a memory. The player should feel that even a catastrophically failed run was worth having. They learned something. They survived long enough to see something new. The Megastructure showed them a piece of itself, and that knowledge carries forward.

---

## 2. NARRATIVE FOUNDATION

### 2.1 The Setting — What Is The Megastructure?

The Megastructure is a structure of impossible scale. It is not a planet. It is not a station. It is something older and stranger than either — an artificial construct of such staggering size that its interior contains its own atmospheric layers, its own gravitational anomalies, and its own forgotten ecosystems that have evolved in the dark over what may be millions of years.

No one knows who built it. No one knows why. The humans who survive within it — the Passengers of The Strata, the scattered settlements at the nodes, the mad engineers who live in the walls — are so far removed from the structure's origin that the question of its purpose has become essentially theological. Some believe it was built as a refuge. Some believe it is a prison. Some believe it is a god.

What is known, in practical terms, is this: The Megastructure is composed of an effectively infinite series of tunnels, chambers, shafts, and voids, all constructed from a material that resembles reinforced concrete but is demonstrably harder, older, and in some places, stranger than any material humanity has encountered before. Some sections glow faintly with residual energy. Some sections are so dark that light itself seems absorbed. Some sections move — slowly, imperceptibly, but measurably, as if the structure is still, somehow, breathing.

The Silicon Life emerged from this environment. They are not invaders from outside. They are the Megastructure's immune system, its janitors, its inheritors — entities that evolved from or were perhaps designed to maintain the structure, and that now treat any human presence as a contaminant to be expelled.

### 2.2 The Strata — Origin & Purpose

The Strata — formally designated Locomotive Unit Iron Strata, nicknamed "The Crawl" by its oldest passengers — is the last mobile human habitat in this region of the Megastructure.

It began as a maintenance locomotive, one of hundreds that once serviced the structure's interior rail networks. At some point in the distant past — accounts differ wildly, and most have the quality of legend rather than history — a group of humans commandeered one of these locomotives and began attaching living quarters to it. Wagons were added. People were born on it. People died on it. Generations passed. The original maintenance locomotive is now buried somewhere in the middle of the train, so deeply encrusted with additional wagons, jury-rigged systems, and decades of improvised engineering that no one is entirely sure where it ends and the additions begin.

The Strata moves because it must. Staying still means the Silicon Life converges. Staying still means the Rations run out. Staying still means the passengers slowly go mad from the pressure of the dark. Motion is survival. The destination is a matter of theology and desperate hope.

### 2.3 The Bark Industry — The Myth

The Bark Industry is a legend. Or it is a signal. Or it is a destination. Depending on whom you ask, it is all three.

Among the passengers and scattered settlements of the Megastructure, there are whispered accounts of a place — sometimes called The Bark, sometimes The Industry, sometimes The Origin — where the air is clean, the food is abundant, and the Silicon Life do not reach. A place where the Megastructure's original purpose is still being fulfilled, whatever that purpose might be.

The signal exists. This much is verifiable. The Strata's navigation systems — ancient, partially functional, requiring constant interpretation — occasionally receive a broadcast on a frequency that does not appear in any of the documentation salvaged from the old maintenance logs. The broadcast is a sequence of tones. Pilots who have studied it their entire careers disagree about whether it is a navigation beacon, a warning, or a language.

The Bark Industry serves as Iron Strata's narrative horizon. It is the reason The Strata moves forward rather than in circles. It is the reason Pilots choose the paths they choose. Whether it is real is a question the game does not answer in a single run. Whether the search for it is worth the cost is a question the player answers every session.

### 2.4 Timeline & Historical Context

**The Deep Past (Unknown):** The Megastructure is constructed. The Silicon Life emerge or are created. No human records exist from this period.

**The Seeding (Unknown, estimated thousands of years ago):** Humans appear within the Megastructure. Whether they were brought here, built here, or wandered in from outside is unknown. The oldest recovered texts suggest multiple independent origin stories that cannot be reconciled.

**The Node Wars (Several centuries ago):** Human settlements at stable nodes across the Megastructure engaged in a series of devastating conflicts over resources and territory. Most of the large, organized settlements were destroyed, either by conflict or by the Silicon Life taking advantage of the chaos. The Strata's oldest wagon-sections date to this period.

**The Scattering (Roughly 200 years ago):** The last major human coalition collapsed. Survivors dispersed into small groups — some mobile like The Strata, some settled in defensible nodes, some simply lost. Communication between groups became sporadic and then mostly ceased.

**The Present:** The Strata rolls. The Silicon Life press. The signal continues. Somewhere, perhaps, the Bark Industry waits.

---

## 3. WORLD-BUILDING & LORE

### 3.1 The Megastructure — Zones & Environments

The procedurally generated tunnels of Iron Strata are organized into environmental zones that affect aesthetics, enemy composition, available resources, and passive hazards. These zones blend at their edges, producing transitional sections that combine elements from both.

**The Transit Corridors** are the most common zone type — massive rectangular tunnels designed for the movement of large vehicles and cargo. The rails run through the center. The ceiling is high enough that even a fully stacked train rarely comes within a hundred meters of it. The walls are relatively smooth, broken by access hatches, maintenance alcoves, and the rusted remnants of signage in a language no one reads anymore. Enemies here favor direct assault. Resources are moderate. Lighting is low but not absent — residual energy still powers a fraction of the original corridor lighting, producing long strips of amber illumination that flicker and die.

**The Void Chambers** are vast open spaces of terrifying scale — spaces so large that the far walls are invisible even in good lighting conditions, and the ceiling disappears into atmospheric haze. These chambers were presumably used for large-scale storage or assembly. Now they are hunting grounds. The open space means enemies can approach from any angle, including above, but also means the train has room to maneuver and that certain long-range weapon wagons become drastically more effective. Resources are rare in the chambers themselves but dense in the structures along the walls — collapsed gantries, ruined storage towers, ancient loading bays — that can be accessed via side branches before entering.

**The Compression Zones** are the opposite of the Void Chambers — tunnels so narrow that The Strata barely fits, where the walls are close enough to touch from an open wagon door. These sections are claustrophobic and tense. Enemies cannot flank. The train cannot be attacked from above. But the tight quarters mean that any structural damage is catastrophic, wagons cannot be repositioned, and the sense of being buried alive is constant and oppressive. These zones are good for certain ambush-based defense strategies but punishing for players who rely on flexible response.

**The Bioluminescent Networks** are sections of the Megastructure where something — biological, chemical, or structural — causes faint light to emanate from the walls themselves. These sections are hauntingly beautiful and deeply unnerving. The light is cold, bluish-green, and casts no shadows properly. Enemies in these sections have adapted to the ambient light and move differently — more cautiously, more intelligently, as if the light affects their behavior as much as the players'. Resources in bioluminescent networks include unique biological components not found elsewhere.

**The Flooded Sections** are tunnels and chambers where ancient piping has failed, filling the lower levels with a dark, cold liquid that may or may not be water. The Strata moves through on elevated tracks, but the liquid surface is close and occasionally breaches the lower wagon levels. Enemies in flooded sections often attack from below, from the liquid itself, suggesting aquatic adaptations. Flooded sections are the source of certain rare resources and are also where certain passenger types — engineers, biologists — can make observations that unlock lore fragments.

**The Ruins Nodes** are what remain of the human settlements destroyed in the Node Wars. They are structurally unstable, heavily colonized by lower-tier Silicon Life, and rich in salvageable materials, old technology, and survivor populations that may be recruited. Approaching a Ruins Node is a risk-reward decision — the resources are valuable, but the structural instability means that certain events can trigger collapses that damage the train.

### 3.2 Silicon Life — Biology & Classification

The Silicon Life are not machines in any conventional sense. They are not robots. They are organisms — evolved or designed, the distinction may be meaningless at this point — that use silicon-based biological processes rather than carbon-based ones. They do not have blood. They have a conductive fluid. They do not breathe. They process ambient electrical energy. They do not reproduce sexually. They construct new individuals from raw materials using a process that looks disturbingly like manufacturing.

This distinction matters for the game because it shapes how players understand and respond to them. Silicon Life are not enemies that feel pain or fear. They do not have survival instincts in any meaningful sense. They have objectives. When they attack The Strata, they are not angry or hungry. They are performing a function — the same function they have been performing for potentially millions of years. The train is a contaminant. The passengers are contaminants. Removal is the objective.

This makes them implacable in a way that is different from, and more unsettling than, a conventional monster. They do not flee. They do not negotiate. They do not tire. They can be destroyed, but new ones will come, built from the same raw materials that everything in the Megastructure is built from. Fighting the Silicon Life is not winning a war. It is managing a condition.

**Classification System:**

Silicon Life are organized by the players and the game's lore into a classification system that reflects their size, function, and danger level. This classification was developed by the passenger-scholars of The Strata and is imperfect — there are entities that do not fit cleanly into any category, and the system has been revised multiple times.

_Class I — Crawlers:_ The most common Silicon Life encountered in Transit Corridors and low-level nodes. Small, roughly humanoid in shape though with proportions that are slightly wrong — limbs too long, joints bending the wrong way, heads that are vestigial at best. They move in swarms. Individually, they are manageable. In numbers, they can overwhelm unprepared defenses through sheer attrition. Their primary attack is direct physical contact, attempting to puncture or rip at wagon exteriors. They are the silicon equivalent of bacteria — ubiquitous, persistent, and dangerous only in concentration.

_Class II — Climbers:_ Larger than Crawlers, with strong magnetic adhesion systems that allow them to traverse vertical surfaces and ceilings. Climbers represent the first genuine tactical threat, as they can approach from the 3D environment — descending support pillars, dropping from overhead structures, emerging from wall-mounted access points above the train's roof. They are more intelligent than Crawlers in the sense that they appear to select targets deliberately, focusing on structural weak points and passenger-occupied wagons rather than hitting the train indiscriminately.

_Class III — Architects:_ A fundamentally different kind of threat. Architects do not attack The Strata directly. They construct. Moving ahead of the train through alternative routes, they build structures — barricades, weapon emplacements, trap systems — that the train then encounters. Fighting the Architects means dismantling their work before it becomes a permanent obstacle while simultaneously dealing with the Crawlers and Climbers that defend the construction.

_Class IV — Sentinels:_ Rare, large, and dangerous. Sentinels are the Silicon Life's heavy units — massive constructs that can sustain enormous amounts of damage and deliver it in kind. Encountering a Sentinel is a significant event. They are not common enemies; they appear as mini-bosses at certain node types and as standard enemies in the deepest tunnel sections.

_Class V — The Unknown:_ There are accounts in the passenger records of encounters with Silicon Life that do not fit any of the above classifications. Entities that are very large, very old, and that behave in ways that suggest something beyond simple objective-completion. The passengers call them Observers. The game does not confirm or deny whether they are a genuine classification or a legend.

---

---

# PART TWO — CORE SYSTEMS

---

## 4. CORE GAME LOOP

### 4.1 The Macro Loop — A Single Run

A single run of Iron Strata follows a repeating structure of movement, decision, and survival. The macro loop consists of:

**Phase 1 — Navigation:** The player views the Node Map, which shows the immediate path ahead. Nodes are visible one or two steps ahead depending on the active Pilot. Each node has an icon indicating its type — Combat, Resource, Settlement, Rest, Mystery, or Boss. The player selects a path, choosing between risk and reward with the information available.

**Phase 2 — Transit:** The Strata moves between nodes. Transit periods are not idle — this is when the player should be managing their wagon configuration, assigning passengers, processing recently acquired resources, and reviewing the train's status. Transit periods have variable length determined by procedural generation. Short transits mean little time to prepare. Long transits are opportunities, but also risk — the longer the transit, the higher the probability of a random encounter.

**Phase 3 — Node Event:** Upon arriving at a node, an event occurs. Combat nodes trigger a siege. Resource nodes offer extraction mechanics. Settlement nodes present dialogue and trading opportunities. Rest nodes allow passenger recovery. Mystery nodes are wildcards — anything from a cache of ancient technology to a hostile ambush to a lore-revealing discovery.

**Phase 4 — Post-Event Management:** After the node event, the player deals with consequences. Repair damaged wagons. Process harvested Scrap. Distribute Rations. Reassign passengers who have been affected by the event. Make new wagon placement decisions if new wagons were acquired. This phase is where much of the game's strategic depth lives.

**Phase 5 — Return to Navigation:** The loop begins again.

### 4.2 The Micro Loop — A Combat Encounter

During a combat node, the micro loop activates. This is the real-time layer of Iron Strata.

The train continues moving — not toward the next node, but through the combat zone. The player cannot stop the train during combat. This is a core design choice that keeps combat dynamic and prevents turtling. The combat zone has a defined length; when the train exits the far end, the encounter ends.

While in combat:

The player manages active systems — firing weapon wagons manually where automation is insufficient, activating special abilities on utility wagons, responding to breach events in real time.

The player monitors passenger status — the Megastructure Stress system (see Section 8) means passengers can deteriorate during combat. High-stress passengers perform their wagon functions poorly. A panicking Engineer means repair rates drop. A paralyzed Gunner means a weapon wagon goes dark.

The player makes emergency decisions — if a wagon takes critical damage, the player must choose whether to attempt repair (costs Scrap, takes time, requires an Engineer passenger), to evacuate the wagon (the passengers survive but the wagon goes dark), or to sacrifice the wagon and attempt to jettison it (removes the wagon entirely, potentially dealing damage to pursuing enemies, but the wagon and its passengers are gone permanently).

### 4.3 The Economy Loop

Running parallel to both macro and micro loops is the constant management of the economy. The two primary resources — Rations and Scrap — create a tension that persists throughout every run.

Rations deplete constantly based on the number of passengers aboard. More passengers means more capability but also more consumption. A large crew is powerful but hungry. A small crew is sustainable but fragile.

Scrap is harvested from combat but consumed by maintenance and construction. The train constantly needs repair. New wagons require Scrap to attach and upgrade. The economy of Scrap is one of constant deficit management — there is never quite enough, and every expenditure is a choice between immediate survival and long-term capability.

---

## 5. THE THREE PILLARS — DEEP DIVE

### 5.1 Pillar I — The Living Machine (Spatial Strategy)

The central innovation of Iron Strata's gameplay is that cards — in the traditional deckbuilder sense — have been replaced by physical objects that exist in three-dimensional space.

In a traditional deckbuilder, a player plays a card and it has an effect. The card goes to a discard pile. The spatial relationship between cards is irrelevant. What matters is the card's effect in isolation and in combination with other cards played in the same turn.

In Iron Strata, acquiring a new Wagon is analogous to drawing a card. But playing that wagon means choosing where to physically attach it to The Strata. That decision is permanent — wagons cannot be freely relocated after placement. And the spatial relationship between wagons is not just relevant; it is frequently the most important factor in whether a configuration succeeds or fails.

**The Placement Grid:** The Strata can be visualized as a three-dimensional grid. The horizontal axis represents the length of the train (front to back). The vertical axis represents height (ground level, first elevated level, second elevated level). The lateral axis represents width — but width is constrained by tunnel dimensions, which vary by zone. In a Compression Zone, the train may only be one wagon wide. In a Void Chamber, it could theoretically be three wagons wide.

**Spatial Bonuses:** Certain wagon types generate bonuses based on their neighbors. A Power Conduit wagon generates more power if surrounded on both horizontal sides by wagons that consume power. A Reinforced Frame wagon provides structural integrity bonuses to all wagons within two positions. A Hydroponics wagon produces more food if placed near a Water Reclamation wagon. These interactions create a spatial puzzle layer on top of the resource management layer.

**The Height System:** Height creates asymmetric vulnerability and capability. Ground-level wagons are vulnerable to Crawler attacks from the sides but protected from above. Mid-level wagons are exposed on three sides in open tunnels. Top-level wagons can target airborne enemies and high-mounted emplacements but are vulnerable to sniper equivalents and to the "Structural Sway" instability mechanic.

**Structural Sway:** When the train reaches high speeds or takes heavy fire, the stack can become dynamically unstable. High-stacked trains sway. This is both a visual effect and a mechanical one — swaying trains have reduced weapon accuracy, higher structural stress on vertical connection points, and risk of partial collapse if stability is not managed. Players who build tall must invest in Stabilizer Wagons and Structural Brace upgrades to mitigate this risk.

**The Configuration Meta:** Over the course of many runs, players develop a deep intuition for wagon placement that constitutes the game's equivalent of "building a deck." But unlike a deck, the configuration is physical, persistent within a run, and interactive with the environment. A configuration that performs brilliantly in a Transit Corridor may be dangerously suboptimal in a Void Chamber.

### 5.2 Pillar II — The Human Component (Micro-Society)

Every passenger on The Strata is a person. This is the central premise of the Passenger System, and it is non-negotiable from a design perspective.

In most survival games, population is an abstraction. You have X workers, and they perform Y tasks. The workers are interchangeable. Losing one worker is equivalent to losing another. The player's relationship to the population is managerial and dispassionate.

Iron Strata breaks this convention deliberately and structurally.

Every passenger has a name generated from a deep pool of Megastructure-appropriate naming conventions. Every passenger has a history — a short background generated from modular elements that establishes their origin, their skills, and their personality. Every passenger has a specialty that makes them meaningfully distinct from other passengers. And every passenger has a stress level that reflects how close they are to breaking.

The emotional weight this creates is not a scripted narrative. It emerges from the system. A player may not have any particular attachment to a passenger named Dura Fennwick when they first appear — a former tunnel surveyor with a strong cartography skill and a moderate stress resistance. But after fifteen nodes, after Dura has been the reason the player found two hidden resource caches, after Dura has been reassigned from survey duty to kitchen duty to keep them away from a high-stress wagon and the player has been carefully managing their wellbeing — when Dura's wagon takes a critical hit and the choice appears on screen, the player feels it.

This is the design working correctly. The systems are engineered to produce attachment, and attachment produces stakes, and stakes are what make the permadeath meaningful rather than frustrating.

**The Management Loop:** Beyond the emotional dimension, the Passenger System creates a practical management puzzle. Passengers need Kitchens (Rations production). They need Bunks (rest recovery, stress reduction). They need Med-Bays (injury and illness treatment). They need meaningful work (idle passengers accumulate stress faster). Managing the ratio of productive wagons to support wagons is a constant optimization challenge.

### 5.3 Pillar III — The Brutalist Odyssey (Aesthetic Power)

The aesthetic of Iron Strata is inseparable from its design philosophy. This is not a game that uses its art style as a coat of paint over functional systems. The visual and audio experience is itself a system — one that communicates information, creates emotional states, and serves as the game's primary argument for why this world is worth inhabiting.

**The Brutalist Visual Language:** The Megastructure is built from a vocabulary of raw concrete, exposed rebar, industrial metal, and the absolute absence of organic softness. There are no curves. There is no warmth in the materials. The architecture communicates that it was not built for human comfort or human scale — it was built for something else entirely, and humans are incidental to its purpose.

This visual language is directly inspired by the manga works of Tsutomu Nihei, particularly BLAME!, in which a lone figure traverses an infinitely self-replicating megastructure. The key visual principle inherited from this source is the sense of **scale incongruity** — the contrast between the enormous scale of the architecture and the tiny, fragile scale of human presence within it.

**The Sublime Terror:** The game's lighting design is built around a concept called Sublime Terror — the experience of encountering something so vast and so indifferent that the emotional response combines awe and dread simultaneously. The train is a spark of light and warmth in an infinite cold darkness. The volumetric lighting system is used not just to illuminate but to define the negative space — the darkness outside the light is as important as the light itself. What you cannot see is where the Silicon Life live.

**Contrast as Communication:** The aesthetic works through contrast. The interior of The Strata is warm — amber light, worn wood, the smell (implied by visual texture) of food and humanity. The exterior is cold — blue-white emergency lighting, raw metal, the dark. When a player is inside their train managing their wagons and passengers, the visual language says _safety_. When the camera pulls out to show the train as a whole, tiny against a void chamber wall, the visual language says _vulnerability_. This emotional modulation serves gameplay — it reinforces the psychological texture of each phase.

---

---

# PART THREE — THE STRATA SYSTEMS

---

## 6. THE STRATA — MECHANICAL ARCHITECTURE

### 6.1 The Locomotive

The Locomotive is the heart of The Strata. It cannot be destroyed — a mechanic that is both narrative (the locomotive is ancient and heavily armored) and functional (losing the locomotive ends the run, but the player should never feel that a single hit can cause that). Instead, the locomotive has a Structural Integrity meter that degrades over time and must be actively maintained with Scrap. If the Locomotive's Structural Integrity reaches zero, the run ends — not with a dramatic explosion, but with the quiet, grinding halt of a machine that has finally given out.

The Locomotive determines the train's base speed, acceleration, and maximum wagon capacity. These values can be improved with Locomotive Upgrades — rare acquisitions that represent fundamental improvements to the core vehicle.

**Locomotive Stats:**

_Speed:_ Determines transit time between nodes and movement speed through combat zones. Higher speed means less time exposed to enemies during transit, but also less time to react during high-speed combat encounters. Speed has a complex relationship with Structural Sway.

_Power Output:_ The locomotive generates electrical power that is distributed through the train via a Power Grid system. Every wagon that is powered requires a certain amount from this output. If total demand exceeds output, wagons begin to shut down, prioritized by their position in a player-configured hierarchy.

_Traction:_ Affects the train's ability to navigate difficult track sections — steep grades, damaged rail, flooded tunnels. Low traction in a difficult section forces a speed reduction, which extends transit time and increases ambush probability.

_Integrity:_ The locomotive's own structural health. Unlike wagon integrity, locomotive integrity does not regenerate and is expensive to repair.

### 6.2 Wagon Connection System

Wagons are connected to each other and to the locomotive via Connection Points — physical linkages that have their own structural properties. A Connection Point can be upgraded (increasing its load capacity, allowing heavier wagons to be stacked on top), damaged (reducing the effective integrity of both connected wagons), or severed (catastrophic — the train splits at that point, and everything behind the severance point is lost).

Managing Connection Points adds another layer to the spatial puzzle. Two very heavy wagons stacked on a single base wagon puts enormous stress on the connection between the base wagon and the one behind it. The player must balance the vertical configuration against the horizontal stress distribution.

---

## 7. WAGON SYSTEM — COMPLETE CATALOG

Wagons are the fundamental building blocks of The Strata. They are acquired through combat drops, node events, trading with settlements, and through the Pilot meta-progression unlock system. Each wagon belongs to a Category, has base stats, upgrade paths, passenger requirements, and special properties.

### 7.1 Wagon Categories

**Combat Wagons** provide offensive and defensive capability. They include weapon platforms, shield emitters, and electronic warfare systems. Combat Wagons are the most immediately essential category but also the most resource-intensive.

**Support Wagons** provide the infrastructure that keeps the train functioning and the passengers alive. Kitchens, Bunks, Med-Bays, Power Conduits, and Repair Bays all fall into this category.

**Production Wagons** generate resources — Scrap refineries, Hydroponics units, Water Reclamation systems, and the rare Research Wagons that generate knowledge-based progress.

**Utility Wagons** have specialized functions that do not fit the other categories — Survey Arrays that improve map information, Communication Wagons that allow contact with distant settlements, Vault Wagons that store excess resources safely, and Structural Wagons that support the train's overall integrity.

**Experimental Wagons** are rare, powerful, and often unpredictable. They represent salvaged technology from the deeper Megastructure — devices whose full function is not entirely understood and that may have side effects, beneficial or dangerous, in addition to their primary function.

### 7.2 Combat Wagon Catalog

**The Repeater Platform** is the baseline ranged combat wagon. It fields a medium-caliber automatic weapon system operated by an assigned Gunner passenger. The Repeater Platform fires forward along the train's axis of movement with a limited horizontal traverse. It is reliable, economical, and forms the backbone of most early-run defensive configurations. Upgrade paths lead toward increased fire rate (at the cost of higher Scrap maintenance), extended traverse (at the cost of structural exposure), or armor penetration (at the cost of reduced rate of fire).

**The Flak Battery** specializes in anti-air and anti-ceiling threats. The Flak Battery fires in a radial pattern above and to the sides of the train, making it essential in any zone where Climbers are active or where aerial Silicon Life variants are encountered. A train without a Flak Battery is profoundly vulnerable to top-down attacks.

**The Siege Mortar** is a high-arc, high-damage weapon with slow reload time. It excels at targeting clustered enemy groups at medium range and is particularly effective against Architect constructions — the high explosive payload damages the structures being built before they can be completed. The Siege Mortar is a deliberate and thoughtful weapon; it rewards players who can anticipate enemy positioning rather than those who react to it.

**The Rail Driver** is a long-range precision weapon — effectively a massive railgun that can fire projectiles through multiple enemies or through light cover. The Rail Driver is devastating against Sentinels and other high-health targets but is almost useless against swarms of small enemies. It requires an extremely skilled Gunner and generates significant heat, requiring a cooling management mechanic when used repeatedly.

**The Flare Wagon** is a utility-combat hybrid. It carries launchers for powerful illumination flares, which are essential in the "Panic" mechanic of extended city sieges and in the "Lights Out" zone hazard. Flare Wagons can also carry incendiary rounds — less effective against the silicon biology of standard enemies but highly effective against Architect constructions, which have organic-composite components.

**The Shield Emitter** projects an energy field across a designated zone of the train. The field absorbs a certain amount of damage before depleting and requires time to recharge. Shield Emitters are not passive — they require an active operator to manage their projection angle and to prioritize which incoming damage to intercept. A well-operated Shield Emitter dramatically extends the survivability of the wagons in its protection zone. A neglected one provides essentially no benefit.

**The ECM Wagon** — Electronic Countermeasures — disrupts the coordination signals that allow Silicon Life to operate in swarms. An activated ECM Wagon creates a sphere of signal disruption around the train that causes nearby Class I and Class II Silicon Life to revert to erratic individual behavior rather than organized assault. It does not damage enemies; it confuses them. It is one of the most powerful tactical tools in the game and one of the most situational — worthless against Sentinels and Architects, which do not use coordination signals.

**The Conversion Cannon** is a late-game experimental weapon that fires a beam capable of — in very specific circumstances — disrupting the silicon biology of Class I Silicon Life sufficiently to cause a behavioral override. A successful conversion does not kill the target; it temporarily redirects it against other enemies. Conversions are unstable, often violent, and always temporary, but the tactical disruption they create in a large enemy swarm is extraordinary.

### 7.3 Support Wagon Catalog

**The Kitchen Wagon** is the first critical support wagon every player must acquire. Without it, Rations are consumed from static storage and cannot be regenerated. A Kitchen Wagon assigned a skilled Cook passenger dramatically slows Ration depletion and can, with sufficient raw ingredients, actually generate surplus Rations. The Kitchen is also a social center — passengers who are not on duty often congregate here, which has passive positive effects on Stress management.

**The Bunk Wagon** provides sleeping quarters for a defined number of passengers. Passengers who are not resting accumulate Stress at a standard rate. Passengers who have access to Bunk time regenerate Stress at a meaningful rate. Without Bunk Wagons, crew stress spirals upward continuously, and the player is left with an increasingly dysfunctional crew regardless of how well the combat systems are managed. Bunk Wagons also represent the single mechanic most likely to produce emergent social events between passengers — the shared sleeping quarters of a stressful survival scenario is a setting that generates stories.

**The Med-Bay Wagon** treats injuries and illnesses. Passengers can be injured in combat events (direct damage to their wagon) or through accidents (a low-probability event in high-stress environments or when operating damaged wagons). Injured passengers perform at reduced capacity. Critically injured passengers cannot work at all and must be actively treated or they will die — not as the direct result of enemy action, but as the consequence of neglect. The Med-Bay requires a Medic passenger to function at full capacity; an empty Med-Bay can perform basic treatment at very low efficiency.

**The Power Conduit Wagon** is an infrastructure wagon with no direct function of its own — it simply extends the effective range of the locomotive's power output and reduces power transmission loss over long trains. Long trains without Power Conduit wagons at appropriate intervals suffer significant power loss in the rear sections, causing distant wagons to operate at reduced efficiency.

**The Repair Bay** allows passengers with Engineering skills to perform active repairs on damaged wagons without requiring the train to stop. The base game allows only limited passive repair; the Repair Bay dramatically increases the rate and quality of repairs, allows structural reinforcement, and enables the "Weld" upgrade (see Economy section). A train without a Repair Bay is in a constant race against accumulated damage.

**The Vault Wagon** is a secure storage wagon. It holds significantly more Scrap and Rations than a standard wagon and protects its contents from being lost if the wagon is heavily damaged. Without a Vault, excess resources are distributed across various wagons and vulnerable to being destroyed in combat. With a Vault, the player has a secure reserve that acts as a buffer against the worst outcomes.

### 7.4 Production Wagon Catalog

**The Hydroponics Wagon** is one of the most transformative acquisitions in any run. Without it, Rations come entirely from external sources — found in nodes, traded at settlements, or carried from the start. The Rations supply is therefore finite and subject to disruption. With a Hydroponics Wagon, the player generates Rations continuously as long as the wagon is supplied with water and light. The Hydroponics Wagon changes the game's economy from one of scarcity management to one of capacity management. Getting one early dramatically increases run survivability.

**The Scrap Refinery** takes raw salvage material — the degraded silicon-carbon composite remains of destroyed enemies, the structural debris of damaged environments — and processes it into usable Scrap at a higher efficiency rate than the basic salvage that happens automatically after combat. A Scrap Refinery shifts the combat economy significantly; fights become more profitable, and the chronic Scrap deficit becomes more manageable.

**The Water Reclamation Unit** produces water from atmospheric moisture, condensation, and recycled biological waste. Water is not a primary resource in the game — players do not track water separately — but it is a prerequisite for several production chains. The Hydroponics Wagon requires water. The Kitchen Wagon operates more efficiently with clean water. The Med-Bay requires water for treatment. Without a Water Reclamation Unit, these chains must be supplied by finding water at nodes, which is unreliable.

**The Research Wagon** is the rarest production wagon and the most strategically significant. It generates no physical resource. Instead, it generates Knowledge — a meta-resource that accumulates over the course of a run and can be spent at certain nodes to unlock Blueprint Fragments that carry over into future runs. The Research Wagon requires the rarest type of passenger — a Scholar — and consumes a significant amount of power. It is an investment in future runs rather than the current one, which creates interesting strategic tension.

### 7.5 Experimental Wagon Catalog

**The Resonance Chamber** emits a low-frequency vibration that affects the structural integrity of Silicon Life. The resonance does not immediately destroy targets; it gradually degrades their structural coherence, making them increasingly vulnerable to conventional weapons. The Resonance Chamber effectively functions as a force multiplier — everything that enters its effective range takes more damage from everything else. However, the resonance is indiscriminate; it also affects the train's own structural connections if the wagon is damaged or misconfigured.

**The Memory Archive** is a lore wagon — one of the most narratively rich acquisitions in the game. It contains fragments of information from the old Megastructure — operational logs, historical records, technical documentation — that can be interpreted by Scholar passengers to reveal information about the Bark Industry, the history of the Silicon Life, and the origins of the Megastructure itself. The Memory Archive does not affect gameplay mechanics directly; its value is entirely narrative, and players who engage with it deepen their understanding of the world substantially.

**The Prototype Cannon** is an extremely powerful weapon wagon of unknown original function. It fires a projectile that the game's passenger-scholars cannot fully explain — something between a physical round and a directed energy beam that causes catastrophic damage to high-tier Silicon Life while apparently having minimal effect on Class I entities. It has a very long cooldown and consuming a unique ammunition that must be found rather than crafted. It is, essentially, a boss-fight tool.

---

## 8. PASSENGER SYSTEM — THE HUMAN COMPONENT

### 8.1 Passenger Generation

Every passenger in Iron Strata is generated from a combination of modular elements that produce a person with a distinct identity.

**Name Generation** uses a pool of approximately 4,000 first names and 3,000 family names drawn from a linguistic system designed for the Megastructure setting — names that sound vaguely like they came from a human culture that has been fragmenting and remixing for centuries in isolation. They are not Earth names, but they are recognizably human. Some names have Megastructure-specific elements — references to tunnel designations, node numbers, or silicon-era vocabulary that has been incorporated into naming traditions.

**Background Generation** is handled by a modular system with three components: Origin (where the passenger came from — a specific node type, a trading caravan, a rescue, a birth on The Strata itself), Experience (what the passenger has done — ranging from farming to combat to scholarship to engineering to nothing in particular), and Incident (a single significant event in the passenger's past that shaped them — a collapse they survived, a person they lost, a discovery they made). These three components combine to produce a one-to-three sentence background that is displayed in the passenger's profile.

**Specialty Generation** determines the passenger's primary skill and secondary skill. Primary skills are directly tied to wagon efficiency — a Gunner's primary skill improves weapon wagon performance, an Engineer's primary skill improves repair rates and wagon upgrade speed. Secondary skills are more varied and often provide passive benefits — a Gunner with a secondary skill in First Aid will sometimes stabilize an injured colleague without being assigned to the Med-Bay. These passive secondary skills are often the source of emergent narrative moments.

**Personality Trait Generation** gives each passenger one or two traits that affect their social dynamics and their stress responses. Traits like "Stoic" (slower stress accumulation, lower emotional response), "Empathetic" (stress contagion — absorbs stress from nearby distressed passengers but also provides comfort to them), "Mechanical" (improved performance on any technical task, reduced performance on social tasks), or "Leader" (provides a passive stress reduction bonus to passengers assigned to the same wagon) create a social layer that operates parallel to the functional skill layer.

### 8.2 Megastructure Stress System

Every passenger has a Stress meter that ranges from 0 to 100. At 0, the passenger is calm, focused, and performing at full capacity. As Stress increases, performance degrades. At specific thresholds, behavioral changes occur:

**0-25 — Stable:** No negative effects. Passenger performs normally. Social interactions are positive.

**26-50 — Tense:** Minor performance reduction (approximately 10%). Passenger begins to exhibit trait-dependent behavioral signals — a Stoic passenger shows nothing, an Empathetic passenger becomes quieter, a Leader may become more assertive.

**51-75 — Stressed:** Moderate performance reduction (approximately 25%). Passenger may refuse secondary task assignments. Social events become more likely — arguments, confrontations, moments of solidarity. The player may receive notifications about specific passengers in this range.

**76-90 — Critical:** Severe performance reduction (approximately 50%). Passenger is at risk of Stress Events — temporary breakdowns that can remove the passenger from duty for a period of in-game time. Stress Events are not permanent but require active management.

**91-100 — Breaking Point:** The passenger's performance is essentially zero. If the stress meter maxes out and is not addressed within a defined time window, the passenger will leave The Strata — not die, but abandon it at the next node. Permanently. This is the game's second form of permanent loss, distinct from death, and in many ways more emotionally complex. The passenger chose to go.

**Stress Reducers:** Bunk Wagon rest, time in the Kitchen social space, specific passenger interactions (particularly with Leader-trait passengers), certain node events (a particularly successful engagement, a beautiful environmental discovery), and Pilot-specific mechanics.

**Stress Amplifiers:** Combat damage to occupied wagons, witnessing the death of other passengers, being assigned to a wagon that is performing a task misaligned with the passenger's skill, extended periods without rest, and the "Panic" mechanic triggered by extended darkness during city sieges.

### 8.3 Passenger Skills — Complete List

**Combat Skills:**
_Gunnery_ — Primary skill for weapon wagon operation. Determines fire rate modifier, accuracy modifier, and special weapon ability activation rate.
_Targeting_ — A secondary combat skill focused on accuracy and threat prioritization. A passenger with Targeting as a secondary skill can dramatically improve the effectiveness of manually aimed weapons.
_Demolitions_ — Specialized in high-explosive applications. Improves Siege Mortar and Flare Wagon (incendiary mode) performance.
_Electronic Warfare_ — Required for the ECM Wagon. Passive secondary skill improves analysis of incoming Silicon Life behavior patterns, giving brief advance warning of attack directions.

**Engineering Skills:**
_Structural Engineering_ — Primary skill for Repair Bay operations. Improves repair speed and quality. Secondary skill: passive reduction in structural damage accumulation.
_Systems Engineering_ — Primary skill for wagon upgrade procedures. Improves upgrade quality and reduces Scrap cost.
_Locomotive Engineering_ — Rare and extremely valuable. Primary skill improves locomotive performance stats. Secondary skill allows the player to perform locomotive repairs mid-run without requiring a full stop.
_Experimental Engineering_ — Required to safely operate Experimental Wagons. Without this skill, Experimental Wagons carry a higher failure rate.

**Life Support Skills:**
_Cooking_ — Primary skill for Kitchen Wagon. Improves Ration production efficiency and quality.
_Medicine_ — Primary skill for Med-Bay Wagon. Improves treatment speed and success rate.
_Hydroponics_ — Primary skill for Hydroponics Wagon. Improves yield and adaptability to different environmental conditions.
_Water Engineering_ — Primary skill for Water Reclamation Unit.

**Navigation & Survey Skills:**
_Cartography_ — Secondary skill primarily. Improves the quality of node map information displayed to the player — more detail about what type of event awaits at the next node.
_Geology_ — Secondary skill. Improves resource identification in mining and salvage contexts.
_Pilot Interface_ — Very rare. Required to take full advantage of Pilot-specific mechanics. Without at least one passenger with Pilot Interface, some Pilot abilities are reduced in effectiveness.

**Social & Management Skills:**
_Leadership_ — Secondary skill. Provides stress reduction aura to co-assigned passengers. Active use in settlements improves trading terms.
_Scholarship_ — Required for Research Wagon and Memory Archive. Produces Knowledge resource.
_Logistics_ — Improves resource processing efficiency across all production wagons when assigned to a central management role.

---

## 9. PILOT SYSTEM — CHARACTERS & META-PROGRESSION

### 9.1 The Pilots — Overview

The Pilot is the character the player embodies. Each run is a Pilot's run — a specific individual with a specific history, a specific set of skills, and a specific relationship to The Strata, its passengers, and the Bark Industry myth.

Pilots are not passengers. They do not occupy a wagon. They are the strategic intelligence of The Strata — the voice in the meta-narrative, the character whose development carries across runs through the meta-progression system. A Pilot cannot die in the conventional sense; if a run ends, the Pilot escapes, or is recovered, or is assumed dead but their knowledge persists. The meta-progression is framed as the Pilot's accumulated experience and the legacy they leave for themselves.

The player begins with two Pilots available: Rybark and Croark. Additional Pilots are unlocked through meta-progression.

### 9.2 Rybark — The Engineer

**Background:** Rybark is the third generation of their family to serve as Pilot of The Strata. Their grandparent was one of the people who first formalized the Pilot role; their parent refined it. Rybark inherited not just the position but a vast collection of technical documentation, engineering notes, and half-completed designs that their predecessors never had time to implement. They are methodical, pragmatic, and deeply invested in the physical structure of The Strata itself. For Rybark, The Strata is not just a vehicle — it is a family heirloom of extraordinary importance.

**Playstyle:** Rybark is the optimization pilot. They are for players who want to build efficient, powerful train configurations and push them to their limits. Rybark's abilities lean toward engineering — faster wagon acquisition, cheaper upgrades, better structural performance.

**Rybark's Unique Mechanics:**

_Blueprint Heritage:_ Rybark starts each run with a selection of Blueprint Fragments from the meta-progression pool. These fragments allow the player to begin with a more advanced wagon configuration than the default start, at the cost of fewer resources.

_Structural Intuition:_ Rybark provides a passive bonus to all Engineering-skilled passengers aboard — their presence as Pilot is the equivalent of a master engineer overseeing the work. This manifests as a flat percentage improvement to all repair and upgrade operations.

_The Family Archive:_ Rybark can unlock (through meta-progression) access to their family's engineering records — extremely detailed documentation of previous train configurations and the results they produced. This functions as an in-game knowledge base that surfaces relevant information when the player is in certain situations.

_Emergency Protocols:_ Rybark's unique active ability. Once per run, Rybark can trigger an emergency protocol that temporarily overrides all wagon power restrictions — every wagon operates at maximum capacity regardless of the Power Grid's actual output. This lasts for a limited duration and causes significant power system stress afterward, but it can turn the tide in a desperate combat encounter.

**Rybark's Meta-Progression Tree:** Rybark's blueprint fragments expand over successive runs. Initial unlocks are modest improvements — slightly better starting weapons, slightly more efficient support wagons. Later unlocks include unique wagon designs that are only available to Rybark runs.

### 9.3 Croark — The Navigator

**Background:** Croark came to The Strata as a stranger — found in a sealed maintenance compartment during a particularly unusual node event, in a state of suspended animation. They do not know how long they were in the compartment. They do not know why they were placed there. They have no memory of the time before, but they have an extraordinary ability: they can read the Megastructure. Not with instruments, not with maps, but intuitively — as if some part of them remembers being here before in ways their conscious mind cannot access. Croark is quiet, observant, and deeply uncertain of their own identity, but possessed of a calm certainty about direction that no one on The Strata can fully explain.

**Playstyle:** Croark is the exploration pilot. They are for players who want to understand the Megastructure more deeply, find better routes, discover hidden nodes, and gradually pull back the map of the world. Croark's abilities lean toward navigation, information, and discovery.

**Croark's Unique Mechanics:**

_The Navigator's Eye:_ Croark reveals 2 nodes ahead on the map instead of the standard 1. This seems like a small advantage but is strategically enormous — it allows planning two moves ahead rather than one, which is the difference between reactive survival and proactive strategy.

_Instinct Mapping:_ Over the course of a run, Croark's growing intuitive understanding of the current Megastructure region builds an Instinct Map — a supplementary map layer that reveals the probability of encountering certain events or resources in different tunnel types. It grows more accurate as the run progresses.

_The Dormant Knowledge:_ Croark's deep, unremembered knowledge of the Megastructure surfaces occasionally as intuitive alerts — warnings before ambushes, guidance toward hidden resource caches, and occasional direct lore revelations that hint at their mysterious origin. These are never guaranteed but become more frequent as runs progress.

_The Signal:_ Croark's unique relationship with the Bark Industry signal. Croark does not just receive the signal; they can, with enough run progress, begin to interpret it — not fully, not reliably, but meaningfully. Runs with Croark have a higher probability of narrative lore events related to the Bark Industry and a higher probability of approaching the true source of the signal.

**Croark's Meta-Progression Tree:** Croark's map expands over successive runs. Each run reveals new map regions that remain permanently visible in the meta-map — allowing future runs with any Pilot to have better awareness of the world. Later unlocks include unique navigation tools and the gradual piecing together of Croark's personal history.

### 9.4 Additional Pilots — Unlockable

**Fenn the Salvager** — unlocked through mid-game meta-progression. Background: Fenn is a professional scavenger who operated independently before joining The Strata as Pilot. Their expertise is in finding value in nothing — in the wreckage, the ruins, the combat aftermath. Fenn's playstyle is economy-focused: every combat encounter yields more Scrap, every ruins node yields more salvage, and Fenn's unique ability to perform real-time battlefield salvage creates a combat-economy hybrid playstyle where the player is incentivized to prolong fights slightly to maximize harvest.

**Dael the Diplomat** — unlocked through late-game meta-progression. Background: Dael was a representative of one of the remaining settled node communities — a diplomat in a world where diplomacy mostly means surviving long enough to agree on anything. Dael's playstyle is network-focused: settlements offer dramatically better trading terms, new passenger recruitment is easier and produces higher-quality results, and Dael's unique ability — the Network Call — can request aid from distant settlements at critical moments, summoning supply drops or defensive support that arrives with a delay.

**Vox the Scholar** — unlocked through the Research Wagon meta-progression path. Background: Vox was the head of the largest remaining repository of pre-Scattering knowledge — a library wagon that was The Strata's most precious resource before it was destroyed in a devastating node event. Vox survived. They dedicated themselves to recovering what was lost. Vox's playstyle is knowledge-focused: Research Wagons generate Knowledge at significantly higher rates, lore events are more common and more detailed, and Vox's unique ability involves applying ancient technical knowledge to jury-rigged engineering solutions that produce improbable but effective results.

---

---

# PART FOUR — ENEMIES & COMBAT

---

## 10. ENEMY FACTIONS — THE SILICON LIFE

### 10.1 Enemy Design Philosophy

The Silicon Life are designed around three core principles that distinguish them from generic game enemies:

**Principle 1 — Functional Purpose, Not Malice.** Every Silicon Life unit has an implied function within the Megastructure's maintenance system. Their attacks on The Strata are not expressions of aggression; they are the execution of a maintenance protocol. This shapes their visual design, their behavioral patterns, and the way the game's lore discusses them.

**Principle 2 — Environmental Integration.** Silicon Life are not imported from outside; they emerge from the environment. Their designs incorporate elements of Megastructure architecture — they look like things that grew out of concrete and metal rather than things that arrived from somewhere else.

**Principle 3 — Adaptive Escalation.** The Silicon Life adjust their tactical approach based on the player's configuration. A player who relies heavily on frontal weapons will face enemies that flank more aggressively. A player who builds tall will face more top-down attackers. This adaptation is not randomized but follows internal logic — the Silicon Life are responding to the same threat assessment protocols that govern all their behavior.

### 10.2 Enemy Behavior Systems

**The Swarm Intelligence Protocol** governs Class I and Class II Silicon Life behavior. Individual units communicate constantly with nearby units using short-range coordination signals. This communication allows swarms to collectively assess threats, coordinate attack angles, and avoid obvious kill zones. The ECM Wagon directly disrupts this protocol.

**The Construction Protocol** governs Class III Architect behavior. Architects operate on long-term construction timelines — they begin preparing obstacles and fortifications well in advance of The Strata's arrival, based on an apparent ability to predict train routing. Players who vary their routes (possible with Croark's navigation bonuses) can sometimes catch an Architect mid-construction.

**The Sentinel Directive** governs Class IV Sentinel behavior. Sentinels are independently intelligent in a way that lower-class Silicon Life are not. They assess individual wagons as targets, prioritizing the most strategically significant (weapon wagons, the locomotive) over the merely accessible. Fighting a Sentinel requires target prioritization and tactical positioning, not just overwhelming firepower.

### 10.3 Enemy Types — Detailed Breakdown

**Silicon Crawler (Class I):** Basic combat unit. Pack tactics. Attempts to breach wagon exteriors through sustained physical contact. Can be cleared efficiently by Repeater Platform wagons with sufficient fire rate. Primary threat in early tunnel sections and outer node perimeters.

**Ceiling Hunter (Class II):** Magnetic climber variant. Approaches from above and from vertical surfaces. Deploys a crushing attack when directly above a wagon. Priority target for Flak Battery wagons.

**Wall Runner (Class II):** High-speed flanking variant. Moves along side walls at significant speed to find wagon weak points not covered by weapon wagons. Requires full weapon traverse coverage to reliably neutralize.

**Void Stalker (Class II):** A variant encountered in Void Chambers. Uses the enormous space to approach from extreme angles — high arcs, wide flanking loops. Detection is the primary challenge; the Searchlight Passenger specialty is designed to address this.

**The Builder (Class III — Architect variant):** The basic construction unit. Individually weak. Deploys simple barriers and wall-mounted weapon emplacements. Priority is disrupting construction before completion.

**The Welder (Class III — Architect variant):** Repairs and reinforces existing structures. Must be prioritized when attacking Architect constructions.

**The Architect Prime (Class III — Architect variant, rare):** A large, powerful Architect unit that directs the construction of sophisticated traps and fortifications. Destroying an Architect Prime immediately stops all ongoing construction in its area of influence. A high-priority, difficult target.

**The Iron Sentinel (Class IV):** The standard heavy unit. High health, moderate speed, capable of destroying a standard wagon in a limited number of hits if uncontested. Requires focused fire from multiple weapon wagons.

**The Siege Engine (Class IV — rare):** A very large Class IV unit that appears only in late-game nodes and city sieges. Carries its own weapon emplacements and deploys Class I units from internal compartments. Fighting a Siege Engine is a multi-phase event.

---

## 11. COMBAT & DEFENSE SYSTEMS

### 11.1 The Adaptive Wave System

The combat encounter system in Iron Strata is built on a dynamic threat director that manages enemy spawning and behavior to maintain a consistent level of tension without becoming predictable.

**The Threat Budget** is the core mechanic. Every combat encounter has a Threat Budget — a value that determines the total enemy force available for that encounter. The Threat Director spends this budget dynamically, deploying enemies in waves based on how the current wave is performing.

If the first wave is being efficiently destroyed by the player's front-facing weapons, the Threat Director will spend budget on Ceiling Hunters and Wall Runners for the second wave, targeting underprotected angles. If the front-facing weapons are being overwhelmed, the Threat Director may continue frontal pressure rather than diversifying.

This creates a combat experience that feels responsive rather than scripted. Players who have covered all angles find the encounters evolve to test their efficiency. Players who have obvious weak points find those weak points exploited.

**Dynamic Entry Points** ensure that enemies do not simply spawn in a predictable location. The tunnel environment is used — enemies emerge from access hatches, descend from ceiling structures, rise from floor grates, drop from passing overhead infrastructure. The environment geometry is always a factor in combat.

### 11.2 The Panic Mechanic

City Siege nodes are the most intense combat encounters in Iron Strata. They are defined by their duration — unlike standard encounters that end when The Strata exits the combat zone, city sieges continue until a specific objective is achieved (destroying a control node, surviving a defined time period, reaching a specific location within the city).

The Panic Mechanic activates when a city siege extends beyond a threshold duration. Light levels in the combat zone drop progressively. The Megastructure's ambient lighting — never generous — begins to fail, and the silicon enemies' operational effects on the environment suppress The Strata's external lighting. In extreme cases, light drops to effectively zero outside the immediate train interior.

In darkness, weapon wagons cannot acquire targets. The combat encounter effectively pauses — but enemies do not pause. They continue to move, to converge, to prepare. The player must use Flare Wagons to restore illumination, or direct passengers with the Searchlight specialty to manually light the combat zone.

Managing the darkness is a distinct skill set that rewards players who have prepared for it.

---

---

# PART FIVE — ECONOMY & WORLD SYSTEMS

---

## 12. THE ECONOMY OF DESPERATION

### 12.1 Rations — The Ticking Clock

Rations represent the food, water, and basic life support consumed by the passenger population of The Strata. They are the game's primary time pressure — the constant, implacable drain that ensures the player cannot afford to be passive or cautious for too long.

Ration consumption scales directly with the number of passengers. Every passenger consumes one unit of Rations per unit of time (transit and node events both consume Rations; the rate during combat is slightly elevated due to physical exertion and stress eating).

When Rations reach zero, passengers begin to take Stress damage at an accelerated rate. Extended Ration shortage causes passengers to become critically stressed, leave the train, or in extreme cases die of malnutrition — the game's most avoidable form of permadeath, and the one that feels worst because it is entirely preventable.

Rations are found in node events, traded at settlements, and produced by Kitchen and Hydroponics Wagons. Surplus Rations can be stored in Vault Wagons for safety.

### 12.2 Scrap — The Construction Currency

Scrap is the material currency of Iron Strata. It is harvested from defeated Silicon Life, salvaged from ruins nodes, found in resource caches, and generated by the Scrap Refinery Wagon.

Scrap is used for:

- Attaching new Wagons to The Strata (base cost)
- Upgrading existing Wagons (variable cost, increases with upgrade tier)
- Repairing damaged Wagons (proportional to damage level)
- Reinforcing Connection Points
- Locomotive maintenance and repair
- The "Weld" operation — permanently fusing two adjacent wagons to create a structural bond that provides bonuses to both but makes them impossible to individually reconfigure

The chronic shortage of Scrap is a designed feature. There is almost never enough Scrap to do everything the player wants to do. Every expenditure is a prioritization decision. Upgrade a weapon wagon or repair the locomotive? Attach a new support wagon or reinforce existing connections?

### 12.3 Knowledge — The Future Currency

Knowledge is the meta-resource generated by Research Wagons and Scholar passengers. It is not used within the current run — it is spent between runs at the meta-progression interface to unlock Blueprint Fragments, expand Pilot abilities, and permanently improve starting conditions.

Knowledge represents the intellectual capital accumulated by The Strata's academic population — their study of salvaged technology, their analysis of the Megastructure, their decoding of the Bark Industry signal. It is the most valuable long-term resource because it directly improves future runs.

---

## 13. PROCEDURAL GENERATION — THE MEGASTRUCTURE

### 13.1 The Tunnel Generation Algorithm

The Megastructure's tunnels are generated procedurally using a layered algorithm that produces varied, believable environments without requiring handcrafted level design.

**Layer 1 — The Zone Assignment:** The first layer determines which environmental zone each tunnel segment belongs to. Zone transitions are gradual, with blending sections that combine elements from both adjacent zones. Zone assignment is influenced by the current run's seed and by the narrative distance from the start — early runs favor Transit Corridors, while deeper runs surface more exotic zones.

**Layer 2 — The Geometry Generator:** Within each zone assignment, the geometry generator produces the specific tunnel shape, dimensions, and architectural features. Corridor width, ceiling height, the presence of overhead structures, the existence of branching passages, the location of access hatches and structural columns — all are determined here. The output is a spatial description of the tunnel that the 3D engine then populates.

**Layer 3 — The Feature Placer:** The feature placer adds specific architectural elements within the geometry — damaged sections, salvageable objects, Silicon Life nest locations, settlement remnants, environmental hazards. Feature placement follows probability tables that are influenced by the zone type and by the current threat level in the run.

**Layer 4 — The Resource Distributor:** The resource distributor places harvestable resources — Scrap deposits, Ration caches, rare material nodes — following a scarcity curve that ensures resources exist but are never trivially abundant.

**Layer 5 — The Atmospheric Layer:** The final layer applies environmental effects — lighting conditions, atmospheric particles (dust, moisture, bioluminescence), audio-affecting elements (echoes, wind, dripping), and the procedural detail that makes each tunnel feel like a specific place rather than a generic level.

### 13.2 No-Two-Runs Guarantee

The combination of these five layers, seeded differently for each run, ensures that no two Megastructure experiences are identical. More specifically:

Resource distribution shifts between runs, meaning a route that was rich in Scrap in one run may be poor in the next. Enemy composition varies, meaning the counter-strategy that worked perfectly before may be insufficient. Zone sequencing changes, meaning the spatial puzzle of wagon configuration is solved differently each time.

This variability is the game's primary replay driver — the mechanical knowledge a player develops is always applicable, but the specific application is always different.

---

## 14. NODE SYSTEM & WORLD MAP

### 14.1 Node Types — Complete List

**Combat Node:** The train enters a zone of active Silicon Life presence and must fight through. The combat encounter uses the Adaptive Wave System. Rewards are Scrap, occasionally a new Wagon, and map information. Risk scales with run depth.

**Resource Node:** A relatively safe area with harvestable materials. The player has a limited time to extract resources before Silicon Life begin to converge. Resource nodes are often the primary source of production wagon components and rare materials.

**Settlement Node:** A location where a small human community has established a stable (if precarious) existence. Settlements offer trading, passenger recruitment, and information exchange. Trading terms depend on the active Pilot and any relevant passenger skills. Some settlements have unique offerings — rare wagons, specific passengers, lore items.

**Rest Node:** A structurally stable area with no immediate threat. The Strata stops, and all passengers have the opportunity for extended rest — dramatically reducing stress levels across the board. Rest nodes are rare and never guaranteed; when they appear, the player must decide whether to use the full rest opportunity (maximum benefit, time cost) or a quick rest (moderate benefit, minimal time cost).

**Ruins Node:** The site of a former human settlement. Unstable, hostile, and rich in salvage. Ruins Nodes are the highest-risk, highest-reward node type. They have a structural instability mechanic that creates a time pressure — the longer the player spends salvaging, the higher the probability of a partial collapse.

**Mystery Node:** A node with an icon that reveals only that something unusual is here. Mystery nodes are wildcards drawn from a large pool of events — environmental discoveries, unique passenger encounters, technology finds, narrative events, ambushes disguised as opportunities, and occasionally simply beautiful and terrible environmental vistas that have no mechanical effect but serve the aesthetic and emotional design.

**Boss Node — The Gates:** The unique terminal nodes of each major run arc. See Section 15.

### 14.2 The Global Map

The Global Map is the cumulative meta-map that expands across multiple runs, primarily driven by Croark's navigation meta-progression. It does not represent a finite world — the Megastructure is effectively infinite — but it represents the player's growing understanding of the regions they have explored.

Map information is used to make more informed routing decisions. A region marked on the Global Map as "High Silicon Life Activity" tells the player to expect more combat nodes. A region marked "Resource Dense" suggests productive resource nodes. A region marked with the Bark Industry signal icon is a high-priority destination for Croark runs.

---

## 15. BOSS ENCOUNTERS — THE GATES

### 15.1 Gates Overview

The Gates are the major structural barriers of the Megastructure — massive archway constructions of ancient design that divide the Megastructure into zones of increasing depth. To pass through a Gate, The Strata must complete a Boss Encounter — a dramatically scaled-up combat event with mechanics distinct from the standard Adaptive Wave encounters.

Each Gate has a Guardian — a unique and significant Silicon Life entity whose design and behavior reflect the Gate's lore and zone character.

### 15.2 Gate Structure

Every Gate encounter has three phases:

**Phase 1 — The Approach:** The train moves toward the Gate on a long approach track. Silicon Life attacks begin in standard wave form, but with escalating intensity as the Gate grows closer. The approach is a warm-up that also serves to deplete player resources slightly before the main encounter.

**Phase 2 — The Gate Event:** The main encounter. The Guardian activates, and the combat becomes a multi-objective event — the player must destroy specific components of the Gate or the Guardian while managing standard enemy waves and responding to the Guardian's unique attacks. Each Guardian has a distinct attack signature that requires specific counter-strategies.

**Phase 3 — The Passage:** If the Guardian is defeated, the Gate opens. The Strata passes through. The passage event is a significant aesthetic moment — a visual and audio sequence that marks the transition to the deeper Megastructure zone beyond. Passage provides significant rewards and unlocks new content for future runs.

### 15.3 Guardian Catalog

**The Warden of Gate I — "The Scaffold"** is a massive Architect-class entity that has been constructing itself into the Gate structure over what appears to have been centuries. It is, effectively, part of the architecture. The combat involves dismantling its extensions while it constructs new ones and deploys subordinate Architect units to rebuild what is destroyed. The Scaffold is a patience and priority test — overwhelming firepower is less effective than systematic deconstruction.

**The Warden of Gate II — "The Tide"** is a swarm-based Boss — not a single entity but a coordinated mass of Class I and Class II units operating under a unified command signal produced by a central node buried deep in the mass. The Tide looks like a wave of the Megastructure itself moving toward The Strata. Finding and destroying the command node within the swarm while managing the physical assault of the tide requires the ECM Wagon and precise targeting.

**The Warden of Gate III — "The Pilgrim"** is the most unusual Guardian — a single, vast, ancient Silicon Life unit that moves slowly and deliberately. The Pilgrim does not attack The Strata directly; it creates environmental conditions that threaten the train — collapsing tunnel sections, flooding events, darkness events. Defeating The Pilgrim requires understanding its movement patterns and positioning The Strata to take advantage of windows in its environmental attacks.

---

---

# PART SIX — PROGRESSION & INTERFACE

---

## 16. PROGRESSION & UNLOCKS

### 16.1 Within-Run Progression

Within a single run, progression is expressed through:

**Wagon Acquisition:** The train grows. New wagons are added, creating new capabilities and new spatial puzzle challenges.

**Wagon Upgrading:** Existing wagons improve through Scrap investment, becoming more effective and gaining new functions.

**Passenger Development:** Individual passengers gain experience performing their specialties, which manifests as small but meaningful performance improvements. A Cook who has spent many nodes in the Kitchen becomes noticeably better than they were at the start.

**Map Revelation:** The world opens up as nodes are cleared. Each new piece of revealed map information is a form of progress.

**Lore Accumulation:** Narrative discoveries — through the Memory Archive, Mystery Nodes, Scholar passenger research, and Croark's signal interpretation — accumulate within the run and contribute to the player's understanding of the world.

### 16.2 Between-Run Progression (Meta-Progression)

The meta-progression system operates between runs and is the mechanism through which the game rewards continued play without making early runs feel wasted.

**The Pilot Progression Tree:** Each Pilot has a progression tree that expands over successive runs. Most nodes in the tree are unlocked with Knowledge currency. Nodes include new abilities, improved base capabilities, unique wagon access, and narrative content.

**The Blueprint Archive:** Blueprint Fragments, collected through runs (particularly with Research Wagons and the Rybark Pilot), are permanently added to the Blueprint Archive. At the start of each new run, the player can select a small number of blueprints from the archive to begin with — specific wagons, specific upgrades — at a Knowledge cost.

**The Global Map:** Each run with Croark expands the Global Map permanently. This map is accessible in future runs by any Pilot, providing improved routing information.

**The Passenger Registry:** Notable passengers who survive a run and successfully reach deep into the Megastructure are added to the Passenger Registry — a record accessible at the meta-progression interface. These passengers can be optionally recruited into future runs at a Knowledge cost, beginning with their developed skills rather than freshly generated.

**The Bestiary:** Each unique Silicon Life type encountered for the first time is added to the Bestiary — an in-game reference that accumulates over runs and provides tactical information about enemy types that have been studied. A fully developed Bestiary entry reveals the enemy's weak points, behavioral patterns, and the specific counter-strategies that scholars have developed.

---

## 17. UI/UX PHILOSOPHY & INTERFACE DESIGN

### 17.1 Core UI Philosophy

The user interface of Iron Strata must do an extremely difficult thing: present a large amount of complex information without interrupting the player's sense of immersion in the world. The Megastructure is vast and cold and beautiful; a cluttered HUD of blinking icons and percentage bars would destroy the aesthetic.

The solution is what the design team calls **Diegetic Information Architecture** — wherever possible, information is presented as part of the world rather than as an overlay on top of it.

The Strata's status is visible in the train itself — damaged wagons show physical damage, stressed passengers show visible behavioral changes, low rations mean the kitchen wagon shows empty shelves rather than just a number changing. The player should be able to read the state of their run by looking at the train, not by reading numbers.

The non-diegetic interface elements that cannot be avoided — the node map, the resource counters, the passenger management screen — are designed to be visually consistent with the Megastructure aesthetic: dark, utilitarian, printed on what looks like salvaged industrial displays. They look like something that exists in the world, not like conventional game UI.

### 17.2 The Passenger Management Screen

The Passenger Management Screen is the most complex interface in the game. It must allow the player to view all passengers, understand their current status, assign them to wagons, and make decisions about their wellbeing — without becoming a spreadsheet.

The design approach is portraiture. Each passenger is represented by a generated portrait — a face, rendered in the game's aesthetic, that is specific to that individual. The portrait is the primary interface element. Surrounding the portrait are simple, clean indicators: the specialty icon, the stress meter (a simple bar), their current assignment, and their current status (a color-coded ring around the portrait: green for stable, yellow for tense, orange for stressed, red for critical).

Detailed information about any passenger is available on demand — click their portrait to expand their full profile. But the overview screen should give the player an immediate intuitive read on crew health without requiring them to process detailed numbers.

---

---

# PART SEVEN — AUDIO & VISUAL DESIGN

---

## 18. AUDIO DESIGN

### 18.1 The Procedural Soundscape

The audio design of Iron Strata is built on a procedural system that generates the game's soundscape dynamically rather than playing fixed audio tracks.

The core audio layers are:

**The Strata Layer:** The locomotive engine, the wheels on the track, the mechanical sounds of working wagons, the human sounds of passengers. This layer is constant and forms the audio identity of The Strata — the player's home, the warm presence in the cold dark.

**The Environment Layer:** The ambient sound of whatever tunnel type the train is currently traversing. Transit corridors have low bass rumble from distant structural movement, occasional drips, the distant mechanical sounds of Megastructure systems still operating. Void chambers have a vast, cathedral reverb — sounds travel enormous distances and return changed. Compression zones are claustrophobic, the walls close enough that the train sounds different, more immediate, more threatening.

**The Silicon Layer:** The audio of the Silicon Life is distinctive and deeply unsettling. It does not sound biological — it sounds mechanical in the way a factory sounds mechanical, a constant undertone of coordinated industrial process. Individual units have specific audio signatures, and the swarm produces a coordinated sound that rises and falls with the wave system.

**The Signal Layer:** Croark's narrative runs have an additional audio layer — a barely perceptible tone that represents the Bark Industry signal. This tone shifts in frequency and pattern as Croark progresses, and attentive players may notice that it changes in response to certain events and discoveries.

### 18.2 Music Design

Iron Strata does not have a conventional soundtrack. There is no background music playing during gameplay in the traditional sense. Instead, the audio system generates what the design team calls **Industrial Drones** — evolving, generative audio compositions built from the game's mechanical sounds.

The Industrial Drone system takes the ambient sounds of the train and the environment and processes them through a generative music engine that finds harmonic relationships and creates a continuously evolving composition. This composition changes with the game state — faster train speed produces higher-pitched drones, combat encounters shift the harmonic center toward dissonance, rest nodes produce the closest thing to conventional music in the game: a simple, quiet, almost melodic arrangement that emerges briefly and then dissolves back into the industrial ambience.

The effect is that the game always sounds like itself — always sounds like the Megastructure — while also providing emotional and atmospheric guidance through the audio.

---

## 19. VISUAL DESIGN & AESTHETIC DIRECTION

### 19.1 Reference Materials

The primary visual references for Iron Strata:

Tsutomu Nihei's manga works (BLAME!, BIOMEGA, NOiSE) — the foundational aesthetic reference. The sense of scale, the brutalist architecture, the tiny human presence, the detailed mechanical design of both human technology and hostile entities.

The photographs of Brutalist architecture from the mid-20th century — particularly Soviet-era large-scale residential blocks, industrial complexes, and infrastructure projects. The weight of concrete, the repetition of structural elements, the way scale creates an almost abstract quality.

The concept art tradition of "industrial fantasy" — games like Disco Elysium, Dark Souls, and the Stalker series — where environments communicate history, entropy, and the weight of time passing.

The works of photographers who document abandoned industrial facilities — the particular quality of light in spaces that were built for function and have been abandoned to time.

### 19.2 Lighting as World-Building

The lighting design is the single most important visual element of Iron Strata. Everything else serves the lighting.

The Megastructure's ambient lighting is ancient and failing — where it still functions, it produces an amber-orange light that has the quality of something that has been running continuously for millions of years without maintenance. Cold spots where lighting has completely failed are genuinely dark — not game-dark, which usually means slightly dimmed and blue-tinted, but actually dark.

The Strata is the primary light source in almost every environment. Its interior is warm and human. Its exterior lighting is functional and industrial. From a distance, it appears as a moving cluster of warm light in an infinite dark void, and this image — a tiny warm spark crossing an impossible distance of cold architecture — is the image at the center of the game's emotional design.

Volumetric lighting is used extensively to make light sources feel real — light scatters through atmospheric particles, creates visible beams in dusty environments, casts long shadows from complex structural geometries. The lighting never feels clean or perfect; it always feels aged, particulate, struggling.

### 19.3 The Silicon Life Visual Design

The Silicon Life must look like they belong in the Megastructure — like they grew out of it or were built from the same materials. Their visual design draws from:

Structural elements — Silicon Life incorporate forms that reference building materials: load-bearing columns, support trusses, reinforced joints. A Class II Climber's limbs look like structural I-beams that have grown flexible.

Industrial machinery — moving parts, visible mechanical systems, elements that suggest function and process. Silicon Life look like they are doing something specific, not just attacking.

Biological form — despite being silicon-based, they have a biological quality to their movement and proportion. Their gait is wrong in the way that unsettles the human eye — slightly off-rhythm, slightly wrong-jointed, almost but not quite familiar.

The color palette for Silicon Life is cold and dark — dark grey, black, deep blue-green, with occasional illumination from their internal energy systems. They should be difficult to see in the Megastructure's ambient light, which is itself a design choice: they are environmental hazards, not obstacles clearly marked for targeting.

---

---

# PART EIGHT — TECHNICAL & BUSINESS

---

## 20. TECHNICAL SPECIFICATION

### 20.1 Engine & Rendering

**Godot 4 with Forward+ (Vulkan Renderer):** The game is built in Godot 4 using the Forward+ rendering pipeline, which provides access to Vulkan's advanced rendering features. This choice is made for several specific reasons:

The Forward+ pipeline supports Screen Space Indirect Lighting (SSIL), which is essential for the game's lighting design — particularly for the subtle fill lighting inside tunnel environments and the way ambient light from The Strata's interior spills onto nearby surfaces.

Volumetric Fog is natively supported and can be parameterized dynamically, which is required for the procedural atmospheric system that adjusts fog density and color based on zone type and game events.

Vulkan's rendering efficiency allows the scene complexity required for the game's aesthetic — a detailed train, complex geometry environments, large numbers of particle effects, and multiple dynamic light sources — to run at target performance on mid-range hardware.

### 20.2 Logic & Programming Architecture

**C# (Mono) for Core Logic:** All game logic — the AI systems, the economy simulation, the procedural generation, the passenger system — is implemented in C# rather than GDScript. This choice is driven by performance requirements, particularly for the passenger AI systems which require multi-threading to simulate hundreds of individual entities simultaneously without performance degradation.

**Component-Based Data Architecture:** The wagon system is implemented with a strict separation between visual components and logic components. A Wagon's appearance (3D model, materials, animation state) is managed by one component tree; its functional behavior (resource processing, combat effectiveness, passenger assignment) is managed by an entirely separate system. This separation serves several purposes: it allows visual upgrades to be applied without affecting logic, it makes the modular wagon content pipeline cleaner and easier to extend, and it dramatically simplifies the DLC development process.

**Procedural Generation Systems:** The tunnel generation algorithm is implemented as a multi-threaded background process that pre-generates upcoming tunnel segments while the player is engaging with the current segment. This approach eliminates loading screens between tunnel sections without requiring the entire run's geometry to be held in memory simultaneously.

### 20.3 Audio Implementation

**Procedural Audio Engine:** Built as a custom Godot 4 plugin that takes physical simulation data (train speed, wagon operational status, environmental measurements) as inputs and generates real-time audio output. The plugin uses a bank of high-quality source recordings that are blended, pitched, filtered, and spatialized dynamically. The Industrial Drone system is implemented as a secondary layer of this plugin that applies harmonic analysis to the current physical audio state and generates supplementary tonal content.

### 20.4 Performance Targets

Target platform: PC (Windows primary, Linux secondary, macOS on Apple Silicon).
Minimum spec target: RTX 2060 equivalent, 16GB RAM, Ryzen 5 3600 equivalent.
Target framerate: 60fps on minimum spec at 1080p.
Recommended spec target: RTX 3070 equivalent and above for 60fps at 1440p with full volumetric lighting.

---

## 21. DEVELOPMENT ROADMAP

### 21.1 Phase 1 — The Vertical Prototype

**Objective:** Prove the core spatial mechanics are fun. Deliver a playable prototype that demonstrates the train movement, wagon placement, and basic combat loop.

**Deliverables:**

A functional 3D Train model on a fixed procedurally generated track. The locomotive and a basic set of five wagon types (Repeater Platform, Kitchen, Bunk, Basic Repair Bay, Vault). The wagon snapping system fully implemented — placement grid, collision checking, connection point visualization. Visual differentiation between Wagon upgrade levels 1, 2, and 3.

A basic Silicon Crawler AI that navigates the environment and attacks the train. One tunnel zone type (Transit Corridor) with basic procedural generation. A basic combat loop: enemies spawn, approach, attack, die, drop Scrap.

The passenger system at the simplest level: passengers exist, they have a skill, they occupy a wagon, they affect its performance.

No UI polish. Development-quality interface for testing purposes. Performance benchmarking complete.

**Success Criteria:** A developer can play for 30 minutes, find the core spatial decision-making satisfying, and understand the basic economy without documentation.

### 21.2 Phase 2 — The Social Engine

**Objective:** Implement the Passenger System in full, the Pilot system for both starting pilots, and the complete basic UI/UX.

**Deliverables:**

Full passenger generation system including name generation, background generation, specialty assignment, and personality trait assignment. The Megastructure Stress System fully implemented with all threshold behaviors. Bunk Wagon and Kitchen Wagon social dynamics.

Both Rybark and Croark implemented with their unique mechanics. The meta-progression framework (without content) — the structure for the Blueprint Archive, the Pilot Progression Tree, and the Global Map.

The Node Map system with three node types implemented: Combat, Resource, and Rest. Basic Settlement nodes (trading only, no passenger recruitment). One Mystery node pool event.

The Passenger Management Screen in production-quality UI. The Node Map interface. The basic HUD during combat. The Resource counters.

Full audio implementation of the Train Layer and basic Environment Layer. Combat audio complete.

**Success Criteria:** A playtester unfamiliar with the game can play a full run to a Game Over, understand what killed them, and immediately want to try again.

### 21.3 Phase 3 — The Megastructure

**Objective:** Full content implementation. All wagon types, all enemy types, all node types, complete procedural generation, all three Zones implemented, Gate I encounter, and production visual quality.

**Deliverables:**

All Wagon categories and types from the design catalog, with full upgrade paths. Complete enemy catalog through Class III. Gate I — The Scaffold — fully implemented. Full procedural generation across three zone types with full atmospheric layering.

Complete visual implementation: full shader suite (subsurface scattering for biological elements, volumetric fog, SSIL), particle systems for combat effects, environmental details, full wagon visual differentiation across all upgrade tiers. Post-processing suite.

Full procedural audio system including the Industrial Drone engine. Complete ambient environment audio for all three implemented zones.

The full Lore system: Memory Archive wagon function, Scholar passenger research system, lore fragment content. The first arc of narrative content supporting both Rybark and Croark's meta-progressions.

All three additional Pilots (Fenn, Dael, Vox) unlockable and fully functional.

**Success Criteria:** The game is content-complete for Early Access. Total average run length is 90–150 minutes. Replayability testing shows meaningful variation between runs.

---

## 22. MARKET POSITIONING & BUSINESS STRATEGY

### 22.1 Target Audience

**Primary:** Players of Slay the Spire, Hades, and Vampire Survivors — experienced roguelike players who understand the run-based loop and actively seek mechanical depth and replayability.

**Secondary:** Players of Factorio, Oxygen Not Included, and RimWorld — management/simulation players who are drawn to the colony and economy systems and who will find the passenger system emotionally engaging.

**Tertiary:** Fans of Tsutomu Nihei's work and brutalist aesthetic in gaming (Dark Souls, Disco Elysium) who are drawn to the world-building and atmosphere.

**Market Opportunity:** The roguelike genre continues to demonstrate strong commercial performance with hits across all budget levels. Iron Strata's synthesis of spatial strategy, colony management, and roguelike structure occupies a distinctive position that differentiates it from the crowded deckbuilder space while remaining accessible to that audience.

### 22.2 Commercial Strategy

**Early Access Launch:** Releasing in Early Access with Phases 1 and 2 complete and Phase 3 in progress. This approach aligns with the game's roguelike genre expectations — the community responds well to Early Access for this genre, and community engagement during development improves the final product.

**DLC Expansion Strategy:** The Wagon system's modular design enables a clean DLC pipeline. Each expansion pack is framed as a new Wagon category — "The Bio-Organic Expansion" introduces wagons made from harvested biological material from specific zone encounters; "The Void-Tech Wagon Pack" introduces experimental wagons derived from ancient Megastructure technology. Each expansion also adds new enemy variants, new node types, and new lore content.

**Additional Pilot DLC:** Each additional Pilot beyond the base four is a potential character DLC, following the successful model established by Hades and other narrative roguelikes.

### 22.3 Competitive Differentiation

Iron Strata occupies a position no current title holds: the combination of spatial 3D wagon building, genuine colony management with emotional stakes, and the production quality of a high-fidelity brutalist aesthetic in the roguelike format.

The spatial building component differentiates it from all deckbuilders. The permanent passenger loss differentiates it from all colony sims. The procedural roguelike structure differentiates it from all narrative train games. The aesthetic differentiates it from nearly everything.

The closest competitor in concept is perhaps Into the Breach in its sense of spatial puzzle-solving under survival pressure, but Iron Strata's real-time combat, colony management layer, and 3D spatial dimension create a fundamentally different experience.

---

---

# APPENDICES

---

## APPENDIX A — Glossary of Terms

**The Bark Industry:** The mythological destination described in the Bark Industry signal. A place of safety, abundance, and answers, whose existence and location are unverified. The primary narrative motivation of the game.

**Blueprint Fragment:** A meta-progression item that records the specifications of a wagon or upgrade for use in future runs.

**Class I-V:** The Silicon Life classification system, ranging from Crawlers (Class I) to the unknown Observers (Class V).

**Compression Zone:** A tunnel zone type where the walls are extremely close to the train, restricting width but providing lateral protection.

**Connection Point:** The physical linkage between two wagons. Subject to stress, damage, and upgrade.

**The Crawl:** Informal nickname for The Strata, used by longtime passengers.

**Diegetic Information Architecture:** The design approach of presenting game information through elements that exist within the game world rather than as HUD overlays.

**Gate:** One of the major structural barriers of the Megastructure, guarded by a Guardian entity. Passing through a Gate transitions the run to a deeper zone.

**Global Map:** The cumulative meta-map of the Megastructure built across multiple runs, primarily by Croark.

**Industrial Drone:** The generative audio system that produces Iron Strata's soundscape.

**Knowledge:** The meta-resource generated by Scholar passengers and Research Wagons, spent on meta-progression between runs.

**Locomotive Unit Iron Strata:** The Strata's formal designation.

**The Megastructure:** The artificial structure of impossible scale that contains the entire game world.

**Megastructure Stress:** The mental health system tracking each passenger's psychological state.

**Node:** A location on the Node Map where a specific event type occurs.

**Panic Mechanic:** The light-depletion mechanic activated during extended city siege encounters.

**Pilot:** The player character — the strategic commander of The Strata, whose development carries across runs.

**Rations:** The resource representing food, water, and basic life support consumed by passengers.

**Scrap:** The material currency harvested from combat and used for construction, repair, and upgrades.

**Silicon Life:** The silicon-based organisms of the Megastructure that antagonize The Strata.

**Structural Sway:** The dynamic instability mechanic affecting tall wagon configurations at high speed.

**Sublime Terror:** The visual design principle describing the emotional effect of scale incongruity.

**The Strata:** The moving fortress that serves as the game's primary location and the player's vehicle and home.

**Vault Wagon:** A secure storage wagon that protects resources from combat damage.

**Void Chamber:** A tunnel zone type consisting of vast open spaces of extreme scale.

**Weld:** The Scrap-expensive operation of permanently fusing two adjacent wagons for structural bonuses.

---

## APPENDIX B — Passenger Name Pool Examples

The following represent examples of the name generation system's output, illustrating the aesthetic of Megastructure naming:

Dura Fennwick, Crosse Altari, Vennig Salthan, Prael Dusk, Morven Calss, Teth Ironmark, Sulo Nine, Arkam Whitechute, Bren Collari, Pell Vanthar, Sinna Coldshaft, Gura Markolin, Tren Deepwall, Assel Fourgate, Navvi Siltmark, Quelm Ashcore, Boral Tunnwick, Dessa Greyfall, Prim Vaultson, Crenn Darkline.

The naming system draws on phonemic patterns that feel both human and unfamiliar — short syllables, hard consonants, references to Megastructure terminology (Gate, Vault, Shaft, Core, Tunnel, Wall) absorbed into family naming traditions.

---

## APPENDIX C — Node Event Pool (Selected Examples)

**Mystery Node Pool Samples:**

"The Observer" — The train enters a tunnel and discovers, hovering motionless thirty meters off the track, an entity approximately eight meters tall that matches no Silicon Life classification. It observes the train for a defined time period without acting. Then it is gone. No mechanical effect. Pure narrative event. Lore entry added.

"The Garden" — A maintenance node has been colonized over an apparent very long period by a non-silicon biological life form — actual organic plants, growing in the Megastructure's residual light and moisture. A rest event, combined with a resource opportunity (the plants are edible in limited quantities). Provides a Ration bonus and a significant passenger stress reduction. Also adds a lore entry — the existence of carbon-based life here is not expected and is not explained.

"The Warning" — The train discovers a blocked passage. The blockage is deliberate — constructed from train parts that have clearly come from another locomotive. On the blockage is a message in a language no current passenger can read. A Scholar passenger, given time, can translate it. The content of the translation is run-seeded — sometimes a warning about what lies ahead, sometimes a route recommendation, sometimes a fragment of the Bark Industry lore, sometimes simply the last words of whoever left it.

"The Recruitment" — A single passenger is found in a maintenance compartment, alive and in surprisingly good condition, claiming to have been waiting. Their story is implausible. Their skills are extraordinary. The player can recruit them or leave them. If recruited, their background eventually surfaces in social events.

"The Wreck" — The remains of another train. Salvage opportunity (substantial Scrap), lore opportunity (the wreck can be investigated to determine what destroyed it — useful tactical information for what lies ahead), and potentially a survivor rescue. Also functions as a weight moment — a reminder of what failure looks like.

---

## APPENDIX D — Design Considerations for Accessibility

Iron Strata's core design creates several accessibility considerations that must be addressed in the final product:

**Visual Accessibility:** The game's low-light aesthetic and the Panic Mechanic's darkness events must be accompanied by robust visual accessibility options. Colorblind modes, adjustable contrast for key interface elements, and an option to display directional threat indicators even in complete darkness (representing audio cues) are required.

**Cognitive Accessibility:** The system complexity of the game is high. The tutorial design must be iterative and optional — players should be able to turn off tutorial prompts once they feel they understand a system, and should be able to turn them back on. A "Guided Run" option for new players that provides stronger default configuration recommendations reduces the barrier to entry without limiting the depth for experienced players.

**Speed Accessibility:** The game has real-time elements in combat. A "Slow Mode" option that reduces the time scale of combat encounters — available as an optional accessibility setting, not a difficulty reduction per se — accommodates players who need more processing time without penalizing them mechanically.

**Emotional Accessibility:** The permanent passenger death system, while core to the design, can be toggled to a "Legacy Mode" in which passengers who would die instead permanently leave the train — the emotional consequence of loss remains, but without death as the mechanism. This is a content warning consideration as much as an accessibility one.

---

_This document contains the complete design reference for Iron Strata. It is a living document and should be updated as design decisions are confirmed, revised, or expanded during development._

_The Strata rolls. The Megastructure waits. Somewhere, the signal continues._
