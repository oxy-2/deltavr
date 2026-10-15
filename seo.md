practical checklist for getting `oxy2` to show up on google. lookup sheet, not a marketing plan. most of this came out of the moth trackers chat where we got into entity identity and oxygenated.uk.

## the naming mess to clean first

you are currently indexed as `oxy-2` in a bunch of places. the goal is `oxy2` (no hyphen). hyphens get treated as word separators and oxy2 is cleaner to type and search.

live hyphen spots called out in the chat:

- github.com/oxy-2 profile url
- repos: oxy-2/deltavr, oxy-2/oxygenated-tabs, oxy-2/Pom-2-OS, oxy-2/mysite
- hack club lapse profile links back to those github paths

### rename path without nuking links

1. github settings, account, change username `oxy-2` -> `oxy2`
2. github sets redirects on repo links (oxy-2/deltavr should forward to oxy2/deltavr). the bare profile url can be claimed by someone else after you leave it, so update external links fast
3. fix hardcoded urls: package.json homepage/repository, site layout links on oxygenated.uk
4. update hack club / lapse / scrapbook connected github handle

## identity google can actually parse

domain is oxygenated.uk (2 year plan). keep it. do not try to brand the domain as oxy2. tell google the person behind the domain.

paste something like this in the site head:

```html
<title>oxy2 | hardware engineer and vr developer</title>
<meta name="description" content="official portfolio of oxy (oxy2), hardware developer in madeira, portugal. building open source vr headsets, controllers, and deltavr moths trackers.">
```

and the schema block:

```json
<script type="application/ld+json">
{
  "@context": "https://schema.org",
  "@type": "Person",
  "name": "Oxy",
  "additionalName": "oxy2",
  "url": "https://oxygenated.uk",
  "jobTitle": "Hardware Engineer and VR Developer",
  "address": {
    "@type": "PostalAddress",
    "addressRegion": "Madeira",
    "addressCountry": "Portugal"
  },
  "knowsAbout": ["Hardware Engineering", "PCB Design", "Virtual Reality", "nRF52840", "SlimeVR", "Embedded Systems", "Deltavr Moths"],
  "sameAs": [
    "https://github.com/oxy-2",
    "https://lapse.hackclub.com/oxy"
  ]
}
</script>
```

when github becomes oxy2, swap the sameAs url. sameAs is doing the heavy lifting: it tells google all these oxys are one person.

## site content that feeds ai overviews

short portfolio blurbs are invisible to crawlers that want answers. write dense technical notes on oxygenated.uk for:

- deltavr (the headset)
- deltavr moths (the trackers)

use the real words. nrf52840, esb, lsmd6dsv, smolslime, mifa, enig, 90mah. when someone searches those plus vr tracker, your page is the citable source.

footer line for the site: `© 2026 oxygenated.uk by oxy2`

optional flex: silkscreen `designed by oxy2` on the pcb itself. photos of the boards then carry the name.

## profiles and backlinks

keep the exact string `oxy2` in the username or display name, link to oxygenated.uk:

- [ ] github (display name, bio, site link)
- [ ] lapse / hackclub (high domain authority, already indexing you. point it at oxygenated.uk)
- [ ] anything else you actually post on

anchor text rule: from other sites, link as `[oxy2 portfolio](https://oxygenated.uk)` not a bare url and not "my website".

## what not to do

- keyword stuffing oxy2 on one page
- buying backlinks
- waiting without submitting anything

## verify loop

1. search `oxy2` and `oxy2 deltavr` / `oxy2 moths`, note rank
2. `site:oxygenated.uk` to see what is indexed
3. search console url inspection on the homepage after title/schema changes
4. re-request indexing

## notes

- `oxy2` alone is a short string. secondary targets like `oxy2 vr` or `oxy2 hardware` are easier
- if someone needs to reach you via google overview later, the schema person block is what feeds that
- full raw chat on this is in `source/moth_trackers_chat_log.txt` (the seo tangent sits near the end)

-oxy
