   M S B A M L   `   `   `  ����  
StudClient    StudClient.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     N� Title$  Student Client��5   	    N� WindowHeight$	 650Q�6     N� WindowWidth$
 1100Q�63    N� ShowsNavigationUI .  .��5      � -   5         �Name$ MainGrid��5      ��5   
   �� 5	       �� EndPoint$ 0.5,1&�6"    � MappingMode$ RelativeToBoundingBox=�63    �� 
StartPoint$ 0.5,0&�6W   .��5
      �  � Color$ 	#FFBFD7EC��6   	 � Offset$	 0Q�61   � 5      $ White��6   $	 1Q�6-   5      
 � RowDefinitions
 5   
   �� 5      $��24 �6   �� 5      $
��411* �6   �� 5      $��25 �6    � ColumnDefinitions 5   
   �� 5      $	��212 �6   �� 5      $
��835* �6   $	��500��6,   .+�5   
   �� -   5         ��Name$ Projects��5      $
��Left=�6$   $��1��6@   $��Stretch=�6M   $	��212��6i    J� Opacity$	 0.6Q�6u   $��16(�6�   6�   o� 5   
   -   5         o�Name$ MainMenu��5      $��Stretch=�6!   $��0��6?   $��Stretch=�6L    o� 
IsMainMenu . $
 0.75Q�6{   $��2��6�   .@�5      m� -   5         m�Name$ SettingsMenu��5      $��Налаштування��6-   $��24��6C   .@�5      m� -   5        $ ConfigSettings��5      $5��/Налаштування підключення��63   6U   m� 5      -   5        $ AccountSettings��5      $@��:Налаштування облікового запису��64   6\   5      m� 5      -   5        $ QuitMenu��5      $��
Вихід��6)   68   5      �� 5   
   -   5         ��Name$ ProjectTabs��5      $��3,0,3,0q�6*   $��1��6;   $��1��6H   $��Stretch=�6X   $��Stretch=�6t   .@�5       �� -	   5         ��Name$ GeneralInfoTab��5       $��Проект��6.   $	��120��6>   .��5!      � -
   5        $ GiGrid��5!      
 5"      �� 5#      $	��25* �6(   $	��125��65   �� 5$      $	��15* �6(   $��75��65   �� 5%      $	��60* �6(   $	��300��65   .+�5'      �� -   5         ��Name$ ProjectDescription��5'       $��14(�6<   $
��Wrap=�6J   $��Stretch=�6^   $��Stretch=�6|   $��0��6�   $��3,3,3,3q�6�   �� 5(      -   5        $ ProjectInfo��5(       $��14(�65   $��Stretch=�6C   $��Stretch=�6a   $��1��6}   $��3,3,3,3q�6�   &   System.Windows.Controls.DataGrid  5)      -   5          Name$ 
EventsGrid��5)      $   HorizontalScrollBarVisibility$
 Auto=�64   "   VerticalScrollBarVisibility$
 Auto=�6Y   $��Stretch=�6|   $��Stretch=�6�   $��2��6�   $��3,3,3,3q�6�      
IsReadOnly . 6�   5*      5+      �� 5,      -   5        $ 	EventsTab��5,      $��
Івент��6)   $	��120��68   .��5-      � -   5        $ EGrid��5-      
 5.      �� 5/      $	��15* �6(   $��75��65   �� 50      $	��30* �6(   $	��150��65   �� 51      $	��45* �6(   $	��225��65   �� 52      $	��10* �6(   $��50��65   .+�54      �� -   5         ��Name$ 
ThemeLabel��54      $��	Тема:��60   $��0��6@   $��16(�6M   $��10,0,0,0q�6[   $
��Bold"�6m   �� 55      -   5        $ Theme��55       $��14(�6/   $
��Wrap=�6=   $��0��6Q   $��Stretch=�6^   $��Stretch=�6|   $��5,30,3,3q�6�   �� 56      -   5        $ DescriptionLabel��56      $ ��Опис завдання:��66   $��1��6O   $��16(�6\   $��10,0,0,0q�6j   $
��Bold"�6|   �� 57      -   5        $ EventDescription��57       $��14(�6:   $
��Wrap=�6H   $��1��6\   $��Stretch=�6i   $��Stretch=�6�   $��5,30,3,3q�6�   �� 58      -   5        $ 	EventInfo��58       $��2��63   $��Stretch=�6@   $��Stretch=�6^   $��3,3,3,3q�6z   �� 59      -   5         ��Name$ DownloadFile��59      $-��'Завантажити завдання��63   $	��150��6R   $��32��6^   $��3��6j   $
��Left=�6w   $	��Top=�6�   $��10,8,0,0q�6�   �� 5:      -   5        $ 
UploadFile��5:      $	��150��61   $#��Відправити файл��6=   $��32��6W   $��3��6c   $��Center=�6p   $	��Top=�6�   $��0,8,0,0q�6�   �� 5;      -   5        $ OpenChat��5;      $	��150��6/   $��Відкрити чат��6;   $��32��6R   $��3��6^   $��Right=�6k   $	��Top=�6�   $��0,8,10,0q�6�   5<      5=      5>      5@      5A      