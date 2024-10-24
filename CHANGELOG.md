# Changelog

## [1.2.0-rc.1](https://github.com/eclipse-tractusx/policy-hub/compare/v1.1.0-rc.2...v1.2.0-rc.1) (2024-10-24)


### Features

* add imagePullSecrets ([#193](https://github.com/eclipse-tractusx/policy-hub/issues/193)) ([64b2f38](https://github.com/eclipse-tractusx/policy-hub/commit/64b2f380c22b9cdfabda393327a50cf4ace1e579))
* **api-hub:** create open api spec on build ([#209](https://github.com/eclipse-tractusx/policy-hub/issues/209)) ([a3c9d6f](https://github.com/eclipse-tractusx/policy-hub/commit/a3c9d6f121e49b4f02999229b51e4fd1d4772bbc)), closes [#207](https://github.com/eclipse-tractusx/policy-hub/issues/207)


### Bug Fixes

* policy hub  post get policy rules response structuring error value map ([#201](https://github.com/eclipse-tractusx/policy-hub/issues/201)) ([b5a1ec3](https://github.com/eclipse-tractusx/policy-hub/commit/b5a1ec3a09a8c7f2325f4f159aa89ca8429e6d2a))
* update incorrect seeding data and not supported policies ([#202](https://github.com/eclipse-tractusx/policy-hub/issues/202)) ([db5171a](https://github.com/eclipse-tractusx/policy-hub/commit/db5171a65f47ec71ebceac479121391d753cc500))


### Miscellaneous Chores

* release 1.2.0-rc.1 ([8ff0f8b](https://github.com/eclipse-tractusx/policy-hub/commit/8ff0f8bd3b430f4b7c970c3b843100f5abd85869))

## [1.1.0-rc.2](https://github.com/eclipse-tractusx/policy-hub/compare/v1.1.0-rc.1...v1.1.0-rc.2) (2024-07-25)


### Features

* add contract reference policy ([#177](https://github.com/eclipse-tractusx/policy-hub/issues/177)) ([4faa2db](https://github.com/eclipse-tractusx/policy-hub/commit/4faa2db2f0e0286effd0714b7f9be7e8b3aff812))


### Miscellaneous Chores

* release 1.1.0-rc.2 ([30f8bbb](https://github.com/eclipse-tractusx/policy-hub/commit/30f8bbb013efcb6ce4f7c03843f2f033581cb295))

## [1.1.0-rc.1](https://github.com/eclipse-tractusx/policy-hub/compare/v1.0.0...v1.1.0-rc.1) (2024-07-16)


### Features

* **attributes:** align seeding for DateExchangeGovernance ([#166](https://github.com/eclipse-tractusx/policy-hub/issues/166)) ([5b04040](https://github.com/eclipse-tractusx/policy-hub/commit/5b04040da08671344a98ce3d37d086f3874055b1)), closes [#165](https://github.com/eclipse-tractusx/policy-hub/issues/165)
* **helm:** consolidate structure in values.yaml ([#156](https://github.com/eclipse-tractusx/policy-hub/issues/156)) ([79559ef](https://github.com/eclipse-tractusx/policy-hub/commit/79559efa0f4ba13c50e51ac427a54abbbfcdd271))


### Bug Fixes

* **image-build:** change from emulation to cross-compile ([#162](https://github.com/eclipse-tractusx/policy-hub/issues/162)) ([075316a](https://github.com/eclipse-tractusx/policy-hub/commit/075316ab0ac4337982273ce50d9a44382e40b496))
* **policy:** adjust attributeKey for purpose ([#161](https://github.com/eclipse-tractusx/policy-hub/issues/161)) ([6df52f6](https://github.com/eclipse-tractusx/policy-hub/commit/6df52f6f7a78b7378801aa95c6b46655e94832da))


### Miscellaneous Chores

* release 1.1.0-rc.1 ([4f66d8e](https://github.com/eclipse-tractusx/policy-hub/commit/4f66d8e602e50553fb46bb5bfb0f544daa324f59))

## [1.0.0](https://github.com/eclipse-tractusx/policy-hub/compare/v1.0.0-rc.2...v1.0.0) (2024-05-23)


### Bug Fixes

* **helm:** change ingress to work without tls enabled ([fd7a634](https://github.com/eclipse-tractusx/policy-hub/commit/fd7a634125d2f3ae9313f541ffec0c352059c535))


### Miscellaneous Chores

* release 1.0.0 ([f289e1d](https://github.com/eclipse-tractusx/policy-hub/commit/f289e1d20ba8adee5e98de3b089188c8eae39e4d))

## [1.0.0-rc.2](https://github.com/eclipse-tractusx/policy-hub/compare/v1.0.0-rc.1...v1.0.0-rc.2) (2024-05-16)


### Features

* **seed:** update policies to standard ([#135](https://github.com/eclipse-tractusx/policy-hub/issues/135)) ([4b04f7a](https://github.com/eclipse-tractusx/policy-hub/commit/4b04f7a60da6a5039a46bd7daf7f3a6c2b86c89b))


### Miscellaneous Chores

* release 1.0.0-rc.2 ([9ad3b37](https://github.com/eclipse-tractusx/policy-hub/commit/9ad3b3713236395e063e148bd51892f02a69ab3f))

## [1.0.0-rc.1](https://github.com/eclipse-tractusx/policy-hub/compare/v0.1.0...v1.0.0-rc.1) (2024-04-30)


### Features

* **bpnl:** add bpnl policy handling ([#116](https://github.com/eclipse-tractusx/policy-hub/issues/116)) ([421202b](https://github.com/eclipse-tractusx/policy-hub/commit/421202b80a8916a0747c39135905eea0479a2540))
* **helm_db-dependency:** change image tag to get latest minor updates ([a2663c1](https://github.com/eclipse-tractusx/policy-hub/commit/a2663c16e260eb33dd969c36d15a1ea77821efd6))
* **helm:** change image tag retrieval for fallback to appVersion ([39b5b69](https://github.com/eclipse-tractusx/policy-hub/commit/39b5b6953c0a3c72f10c66ecc71b097144ac02ae))
* **helm:** change ingress according to TRG-5.04 ([b00b25f](https://github.com/eclipse-tractusx/policy-hub/commit/b00b25f1cd684bddbaf2461c1405db3321a929d8))
* **helm:** consolidate centralidp configuration ([660ad6f](https://github.com/eclipse-tractusx/policy-hub/commit/660ad6faec42d14a9e5d2f139a6a0488506fb06c))
* **helm:** move health checks to service ([7b92236](https://github.com/eclipse-tractusx/policy-hub/commit/7b9223693fe7a65a33d01fd25ee7c51fa2f788a5))
* **helm:** move passwords for db dependency to according section ([26b0b4e](https://github.com/eclipse-tractusx/policy-hub/commit/26b0b4e688742f59ead334253a19d9715d04729d))
* **helm:** set resource limits ([913c837](https://github.com/eclipse-tractusx/policy-hub/commit/913c837a6750eca2362a6197f67cf60e7a9e1e70))
* **net8:** upgrade to .net8 ([#102](https://github.com/eclipse-tractusx/policy-hub/issues/102)) ([fb9e3c9](https://github.com/eclipse-tractusx/policy-hub/commit/fb9e3c944bbee02f5800ab99095ffb439bc91dd3)), closes [#19](https://github.com/eclipse-tractusx/policy-hub/issues/19)
* **policy:** add value check to post endpoint ([#97](https://github.com/eclipse-tractusx/policy-hub/issues/97)) ([2039af0](https://github.com/eclipse-tractusx/policy-hub/commit/2039af081fe41add04518a0932948e989967dbac)), closes [#68](https://github.com/eclipse-tractusx/policy-hub/issues/68)
* **policy:** policy seeding data update ([#88](https://github.com/eclipse-tractusx/policy-hub/issues/88)) ([10bb931](https://github.com/eclipse-tractusx/policy-hub/commit/10bb931387e93260d84bdacf59f1fcd77b76e169)), closes [#25](https://github.com/eclipse-tractusx/policy-hub/issues/25)
* **template:** policyhub restrict or operand to access policies ([#107](https://github.com/eclipse-tractusx/policy-hub/issues/107)) ([6a4cacd](https://github.com/eclipse-tractusx/policy-hub/commit/6a4cacd607325fecf08ce567c13b2d3bacd2e636)), closes [#43](https://github.com/eclipse-tractusx/policy-hub/issues/43)


### Bug Fixes

* **helm:** fix label and username for external database ([#72](https://github.com/eclipse-tractusx/policy-hub/issues/72)) ([e9817ff](https://github.com/eclipse-tractusx/policy-hub/commit/e9817ffd1d38db9525338e965248f7375a592857))


### Miscellaneous Chores

* release 1.0.0-rc.1 ([9f1cb78](https://github.com/eclipse-tractusx/policy-hub/commit/9f1cb78344b742bf7a834369d99720a6ff5eeaff))

## [0.1.0](https://github.com/eclipse-tractusx/policy-hub/compare/v0.1.0-rc.3...v0.1.0) (2024-03-06)


### Features

* **helm-chart:** define templates for db hostname uniquely ([#41](https://github.com/eclipse-tractusx/policy-hub/issues/41)) ([b4d0d79](https://github.com/eclipse-tractusx/policy-hub/commit/b4d0d79feca3ff0238d22f1f8ba9b8addb461023))
* **helm-chart:** improve ingress, labels and namespace setting ([#45](https://github.com/eclipse-tractusx/policy-hub/issues/45)) ([1789336](https://github.com/eclipse-tractusx/policy-hub/commit/178933624765f7849b2253d24076e58dbac49224))

### Miscellaneous Chores

* release 0.1.0 ([c00c513](https://github.com/eclipse-tractusx/policy-hub/commit/c00c513e7a614245805ca55b63bc76d4eb35b055))

## [0.1.0-rc.3](https://github.com/eclipse-tractusx/policy-hub/compare/v0.1.0-rc.2...v0.1.0-rc.3) (2024-02-15)


### Features

* **nuget:** update framework packages to stable ([#30](https://github.com/eclipse-tractusx/policy-hub/issues/30)) ([c097c90](https://github.com/eclipse-tractusx/policy-hub/commit/c097c905b8280a65065dffa6524d0dd4bda7d0be))

### Bug Fixes

* **helm-chart:** don't change postgres secret at helm upgrade ([#33](https://github.com/eclipse-tractusx/policy-hub/issues/33)) ([b7b4b8f](https://github.com/eclipse-tractusx/policy-hub/commit/b7b4b8fbff2286a4cf12c3783d6f9bff05cf717b))

### Miscellaneous Chores

* release 0.1.0-rc.3 ([6d52a25](https://github.com/eclipse-tractusx/policy-hub/commit/6d52a25d6effcf1f9753249d1f45fa24e0e43208))

## [0.1.0-rc.2](https://github.com/eclipse-tractusx/policy-hub/compare/v0.1.0-rc.1...v0.1.0-rc.2) (2024-02-02)


### Features

* make imagePullPolicy configurable, default set to IfNotPresent ([1586de5](https://github.com/eclipse-tractusx/policy-hub/commit/1586de5d6322a92db28dc2f1e3457091087b965c))
* **trg-4.07:** enable readOnlyRootFilesystem for containers ([9fcc10e](https://github.com/eclipse-tractusx/policy-hub/commit/9fcc10ec743ea3cbeb5a3026e7161789ceee7339))


### Miscellaneous Chores

* release 0.1.0-rc.2 ([ac5cc45](https://github.com/eclipse-tractusx/policy-hub/commit/ac5cc45df8c73556d636b9a18adbf3cda2d39f27))

## 0.1.0-rc.1 (2024-01-23)


### Features

* add initial implementation for policy hub ([0b5433a](https://github.com/eclipse-tractusx/policy-hub/commit/0b5433a989e34a4fce9b12ac0f7ef3a09b2a86d4))
* add purpose trace 3.1 id ([243488a](https://github.com/eclipse-tractusx/policy-hub/commit/243488aece1731481a5aebd67f2b8de961987cbd))
* **helm-chart:** ensure unique resource names ([#11](https://github.com/eclipse-tractusx/policy-hub/issues/11)) ([cd56676](https://github.com/eclipse-tractusx/policy-hub/commit/cd56676f49073a032d0905d5dcb637898d983ec2))
* **helm-chart:** use templates for unique resource names ([#14](https://github.com/eclipse-tractusx/policy-hub/issues/14)) ([d412b38](https://github.com/eclipse-tractusx/policy-hub/commit/d412b389fd45e2aec2e8db20dc64d70f41a2d563))


### Bug Fixes

* set product name in .tractusx ([#3](https://github.com/eclipse-tractusx/policy-hub/issues/3)) ([04b6689](https://github.com/eclipse-tractusx/policy-hub/commit/04b668933812737a691d118662ccdd349a14909b)), closes [#4](https://github.com/eclipse-tractusx/policy-hub/issues/4)


### Miscellaneous Chores

* release 0.1.0-rc.1 ([d24b8a4](https://github.com/eclipse-tractusx/policy-hub/commit/d24b8a426a151addc31b52806e4e4c8a0270741a))
