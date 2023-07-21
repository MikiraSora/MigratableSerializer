# MigratableSerializer
可"迁移更新"的数据结构实现/协议

## 需求简介
比如存在早期数据结构Config Ver.N，然后需要更新到Config Ver.N+1的实现，但考虑到更久远的Config 更新和兼容(甚至考虑到向前兼容)，
那么就需要一个小小的框架和接口去负责它们之间的迁移和兼容了。

此库功能&特色:
* 从Ver.1迁移到Ver.N 的数据结构
* 可以从Ver.N开倒车回Ver.1
* 不需要自己对着一堆版本做判断和hardcode写更新，只需要写好Migration就行
* 对于某个版本的正反序列化，只需要自己写一个IFormatter/IParser即可
