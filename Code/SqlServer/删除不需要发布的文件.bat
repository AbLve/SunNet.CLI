  
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


 del .\assessment\ɾ������Ҫ�������ļ�.bat /q 
 del .\MainSite\ɾ������Ҫ�������ļ�.bat /q 
 del .\sso\Z_Delete_config_files.bat /q 
 del .\Static\ɾ������Ҫ�������ļ�.bat /q 

  del .\Collaborative\ɾ������Ҫ�������ļ�.bat /q 
  
pause