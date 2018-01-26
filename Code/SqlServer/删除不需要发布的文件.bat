  
rd  /s /q .\assessment\bin\zh-Hans
rd  /s /q .\MainSite\bin\zh-Hans
rd  /s /q .\sso\bin\zh-Hans
rd  /s /q .\Static\bin\zh-Hans
rd  /s /q .\Collaborative\bin\zh-Hans

del .\assessment\Web.config /q 
del .\assessment\log4net.config /q 

del .\MainSite\Web.config /q 
del .\MainSite\log4net.config /q 

del .\sso\Web.config /q 
del .\sso\log4net.config /q 

del .\Static\Web.config /q 
del .\Static\log4net.config /q 

del .\Collaborative\Web.config /q 
del .\Collaborative\log4net.config /q 


 del .\assessment\删除不需要发布的文件.bat /q 
 del .\MainSite\删除不需要发布的文件.bat /q 
 del .\sso\Z_Delete_config_files.bat /q 
 del .\Static\删除不需要发布的文件.bat /q 

  del .\Collaborative\删除不需要发布的文件.bat /q 
  
pause