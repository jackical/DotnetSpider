20160127
1. 重构了Selector模块
2. 在没有新的Request之后将不再打印进度(Left永远为 0)
3. 添加FileDownloader, 当实际生产环境的Downloader过程过于复杂的时候(如需要登录，开启WebDriver...), 可以手动

20160128
1. 添加验证模块
2. 修正RedisScheduler当Poll一个Request后, 把item中存的JSON数据也一并删除.