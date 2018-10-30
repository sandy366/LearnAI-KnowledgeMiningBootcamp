# Finished Solution - CusAzure Search Lab

1. Return only the first document: `$top=1`

1. Search documents where words "Microsoft" and "Cloud" are up to 20 words distant one from the other: `"Microsoft Azure" ~ 20`

1. Search for documents about Cloud, ordering the results by the score.
1. Search for documents about Cloud, but filtering those with mentions to Oracle: `"Cloud" -Oracle`
1. Leverage the Analyzer searching for Microsoft, Azure and the vatiations you decide.
