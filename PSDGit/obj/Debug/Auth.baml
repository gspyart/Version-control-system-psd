   M S B A M L   `   `   `  ����A  =PSDGit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:PSDGitPSDGit  5   	       PSDGit.Auth   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:PSDGit     5   	     � WindowStartupLocation$  CenterScreen=�5   	    � AllowsTransparency .  � Title$
 Auth��5
   	    � WindowStyle$
 None=�6   $	��500��5   	   $	��300��6   
������  � 	Resources 5      ��%    5      .��5      ��  �� CornerRadius$ 8��6   $��0q�6   
�������._�5   
   �  � ColumnDefinitions 5      �� 5      6%   �� 5      $��3* �6#   60   �� 5      6%    � RowDefinitions 5      �� 5      6"   �� 5      6"   �� 5      6"   .+�5       �� -   5         ��Name$ 
PGLoginBtn��5       $��2��6*   $��1��67   $��200px��6G   $
��50px��6U   ��5!      � 5"      $��Center=�6    5#      �� 5$      61   �� 5%      61   .+�5'      �� $��Center=�6$   $��1��6?   Log in6X   �� 5(      $��dropbox.png��6!   68   5*      5,      �� 5-      -   5        	 ��Name$	 
PGuserlist��5-      $��Center=�6*   $��1��6E   $��1��6U   ��5.      �� 5/      .R�50      $� 
 K� IsItemsHost
 .  $� Orientation$ 
Horizontal=�6D   51      53      (  "System.Windows.Controls.WebBrowser  54      -   5          Name$ 
PGExplorer��54   0   $y�	Collapsed=�6   $��3��6D   $��3��6U   6k   55      57      59      