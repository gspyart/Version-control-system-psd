   M S B A M L   `   `   `  ����F  BPSDGitFinal, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:PSDGitFinalPSDGitFinal  5   	   %    PSDGitFinal.AuthorizationWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:PSDGitFinal     5   	     � 
ResizeMode$  NoResize=�5   	    � AllowsTransparency .  � WindowStyle$
 None=�5
   	    � Topmost . 
������  � WindowStartupLocation$ CenterScreen=�5   	    � Title$ 
MainWindow��5   	   $	��320��5   	   $	��360��5   	   $��0q�5   	    � 	Resources 5         ClosingButtonAnimation g� FillBehavior �� TargetProperty	 �� 
TargetName9  3System.Windows.Media.Animation.EasingDoubleKeyFrame
 O� KeyTime O� Value ! ClosingButtonAnumationLeaving  ComboBoxToggleButton �� 
TargetType � ColumnDefinitions ��Name �� CornerRadius �� Color F�Name  ComboBoxTextBox �� 
TargetType �� Property �� Value d�Name �� 	ClickMode d� 	IsChecked �� Mode �� RelativeSource ��Name J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate ��Name �� 
IsReadOnly  �� 
IsReadOnly! �Name" �� IsDropDownOpen# � AllowsTransparency$ �Name% J� SnapsToDevicePixels& � ActualWidth' �� MaxDropDownHeight( K� IsItemsHost-  'System.Windows.Input.KeyboardNavigation)   DirectionalNavigation* �� Triggers+ P� Property, P� Value- �� 
TargetName. P� 
SourceName/ �� Padding��%<  &	        &	     &	 �	    &	 �    '�� S    7  7  '�� �    �� 5   
   $ HoldEnd=�64   .��5      U� $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�6,   $	 button��6�   .\�5        $
 0��6'   $ 1Q�63     5      $
 0:0:0.15��6'   $ 1.275Q�6:   5      U� 5      $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�6,   $	 button��6�   .\�5        $
 0��6'   $ 1Q�63     5      $
 0:0:0.15��6'   $ 1.275Q�6:   5      U� 5      $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�6,   $	 button��6�   .\�5        $
 0��6'   $ 0Q�63     5       $
 0:0:0.15��6'   $	 -11Q�6:   5!      U� 5"      $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�6,   $	 button��6�   .\�5#        $
 0��6'   $ 0Q�63     5$      $
 0:0:0.15��6'   $ 0Q�6:   5%      U� 5&      $V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�6,   $	 border��6�   .\�5'        $
 0:0:0.15��6'   $ 0Q�6:     5(      $
 0:0:0.4��6'   $	 -45Q�69   5)      U� 5*      $V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�6,   $	 border1��6�   .\�5+        $
 0:0:0.15��6'   $ 0Q�6:     5,      $
 0:0:0.4��6'   $ 45Q�69   5-      5.      �� 5/   
   .��50      U� $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)�6,   $	 button��6�   .\�51        $
 0:0:0.2��6'   $ 1Q�69   52      U� 53      $V P(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)�6,   $	 button��6�   .\�54        $
 0:0:0.2��6'   $ 1Q�69   55      U� 56      $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)�6,   $	 button��6�   .\�57        $
 0:0:0.2��6'   $ 0Q�69   58      U� 59      $U O(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)�6,   $	 button��6�   .\�5:        $
 0:0:0.2��6'   $ 0Q�69   5;      U� 5<      $V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�6,   $	 border��6�   .\�5=        $
 0:0:0.2��6'   $ 0Q�69   5>      U� 5?      $V P(UIElement.RenderTransform).(TransformGroup.Children)[2].(RotateTransform.Angle)�6,   $	 border1��6�   .\�5@        $
 0:0:0.2��6'   $ 0Q�69   5A      5B      �� 5E   
   " d�5F      .R�5G      �  5H      �� 5I      �� 5J      $��20 �6   .+�5M      �� $ Border��6   $��2��5N      $ 2��5O      $��1q�5P      5R      �� 5S      $��0��6   $ 2,0,0,2��5T      $��1q�5U      ��5V      �� 5W      $ #49465a��6   5Y      F� 5Z      $ Arrow��6   $��1��5[      $��Center=�5\      $��Center=�5]      "����        � 	=  	= � z     	��5_      �� 5`      $ #0264fb��6*   5b      5c      5d      �� 5f      " ��5g      .R�5h      �� $ PART_ContentHost��6   |�.  #��z��5k      5m      5n      �� 5p      " ��5q      .�5r      ��  �@J�SnapsToDevicePixels . 5s      �� 5t       �@�OverridesDefaultStyle . 5u      �� 5v       � ��$
 Auto=�5w      �� 5x       � ��$
 Auto=�5y      �� 5z       � �� . 5{      �� 5|       � ��$	 120��5}      �� 5~       � ��$ 20��5      �� 5�       � �� 5�      �� 5�      " ��6   .R�5�   
   � .+�5�      d� $ ToggleButton��6   8��  $��2��5�      |�.  $ Press=�5�       5�      �� *IsDropDownOpen+$ TwoWay=� �� *TemplatedParent+�� 5�      $ ContentSite��6    .  #��z 5�      #��z 5�      #��z��5�      $��3,3,23,3q�5�      $��Stretch=�5�      $
��Left=�5�      
�������5�      �� 5�      $ PART_EditableTextBox��6   8�� $
��Left=�5�      $��Bottom=�5�      $��3,3,23,3q�5�      |�. 
������ $y�Hidden=�5�      #  z 5�      ��5�      Z� 5�      � 5�      $! Popup��6   $��Bottom=�5�      #��z" 5�      # . |�.  $��Slide=�5�      .��5�      � $$ DropDown��6   % . #��z& 5�      #��z' 5�      .+�5�      �� $ DropDownBorder��6   $��1q�5�      ��5�      �� 5�      $
 Blue��6$   5�      �� 5�      $��4,6,4,6q�6   % . .��5�      �� ( . $) 	Contained=�5�      5�      5�      5�      5�      * 5�   
   P� 5�      + �@��HasItems, .  . �5�      �� $- DropDownBorder��6    � ��$ 95��5�      5�      P� 5�      + �@��
IsGrouping, . . �5�      ��  � �� .  5�      5�      P� 5�      $. Popup��6   + �@�AllowsTransparency, . . �5�      �� $- DropDownBorder��6    �@��CornerRadius$ 4��5�      �� 5�      $- DropDownBorder��6    � ��$ 0,2,0,0q�5�      5�      5�   	   5�      5�      �� 5�      " ��5�      .�5�      ��  �@J�SnapsToDevicePixels . 5�      �� 5�       �@�OverridesDefaultStyle . 5�      �� 5�       � �� 5�      �� 5�      " ��6   .R�5�   
   �� $ Border��6   $/ 2q�5�      % . 
������ ._�5�      �� 
�������6   5�      5�   	   5�      5�      5�      .��5�      � .+�5�   
   �� -   5        $ 	Container��5�      $��1q�6$   
������� 5�      �� *WindowCornerRadius+0 �� FallbackValue$0 10��� 5�   
   1 J� OpacityMask1 5�      !� 5�      2 !� Visual2 6   �� 3 �� ElementName$3 	Container��.+�5�      � ���#1e1c294 � RowDefinitions4 5�      �� 5�      $
��0.2* �6$   �� 5�      $
��0.8* �6$   .+�5�      � $��0��6   .+�5�      �� $��0q�6   $��0 25 0 0q�60   $��Center=�6B   ���#46435b.��6t   PhotoshopGit6�   �� 5�      -   5        5 ��Name$5 
stackPanel��5�   !   $��	100 0 0 0q�65   6 �� Orientation$6 
Horizontal=�6H   $��Right=�6a   .+�5�      �� -   5        7 ��Name$7 button��5�   5   $��0q�6!   $��40��6Z   $��40��6e   $	��Top=�6q   z�5�      V� 5�   "   .~�5�   &   �� �� 5   &   �� 5  &   T� 5  &   5  #   8 � Triggers8 5     :� 5  "   9 :� RoutedEvent$9 
MouseEnter��6/   .[�5  &   �� #��[  66   5  #   :� 5	  "   $9 
MouseLeave��6/   .[�5
  &   �� #��[ 66   5  #   ��5     �� 5  "   " ��6(   .�5  &   ��  � �� �#0062ff6C   �� 5  &    � �� 5  *   �� 5  .   " ��6>   .R�5  2   �� #��z��69   #��z��6Y   $ 	0 0 0 100��6{   #��z��6�   ._�5  6   �� 5  3   5  /   5  '   5  #   .��5     � $��Center=�6#   $��Center=�6@   .+�5  "   �� -   5        $ border1��5  )   $��12��6:   $	��1.5��6E   
���   �$��7 0 0 0q�6e   : J� RenderTransformOrigin$: 0.5,0.5&�6v   z�5  &   V� 5  *   .~�5  .   �� �� 5   .   �� 5!  .   T� 5"  .   5#  +   5%  #   �� 5&  "   -   5        $ border��5&  )   $��12��69   $	��1.5��6D   
���   �$��7 0 0 0q�6d   $: 0.5,0.5&�6u   z�5'  &   V� 5(  *   .~�5)  .   �� �� 5*  .   �� 5+  .   T� 5,  .   5-  +   5/  #   50     51     52     53     �� 54     $��1��6   $��Center=�6*   .+�55     �� $��45(�6    $��0 10 0 0q�6.   $��Center=�6@   
�������Hi, �� 6u   ���#0264fb.�6�   bro6�   !6�   �� 57     $��0 15 0 0q�6    ; �� TextAlignment$; Center=�62   $
��Wrap=�6J   ���#46435b$"Log in to your account to continue�� 6�   %#Best regards, PSDGit developer team6�   �� 58     -   5          AnimatedProjectButton#��[ 58  5   $��0 30 0 0q�59     $��1��60   $	��160��6@   $��35��6L   .��5:     �� $��Center=�6    $��Center=�6;   .��6X   Log in to DropBox5<     5=     �� 5E     $��0 7 0 4q�6    ���#46435b*(Or choose one of already logged account:6p   �� 5F     -   5        < ��Name$< select��5F     = �� DisplayMemberPath$= username��6S   ��6/   �� *ActiveUsers+5H     5I     5J     5K     5L     5M     