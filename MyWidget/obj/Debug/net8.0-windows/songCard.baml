   M S B A M L   `   `   `  ����C  ?MyWidget, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:MyWidgetMyWidget  5          MyWidget.songCard   5      S OHandyControl, Version=3.5.1.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      (localclr-namespace:MyWidget     5      $	��455��5      $	��120��6+   .��5	      �   � RowDefinitions  5
   
   �� 5      $��10 �6   �� 5      $	��20* �6   �� 5      $	��20* �6   �� 5      $	��20* �6   �� 5      $	��20* �6   �� 5      $��10 �6    � ColumnDefinitions 5   
   �� 5      $��10 �6   �� 5      $
��150* �6   �� 5      $	��70* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5      $	��40* �6   �� 5       $��10 �6   .+�5#   
   �� -   5         ��Name$ songNameLbl��5#      $��1��6%   $��2��62   $��Center=�6p   $
��Left=�6�   
���������6C   Z� ��6Y   Z� .��6�   Song5%      �� 5&   
   -   5        $ artistNameLbl��5&      $��2��6'   $��2��64   $
��Left=�6r   
���������6E   Z� ��6[   Z� .��6�   Artist5(      �� 5)   
   $��3��6   $��2��6   $
��Left=�6[   
���������6.   Z� ��6D   Z� .��6�   Album5+      �� 5,   
   $��4��6   $��2��6   $
��Left=�6[   
���������6.   Z� ��6E   Z� .��6�   Link5.      #  HandyControl.Controls.TextBox  50   
   -   5          Name$ songName��50      $��1��6'   $��3��64   $��8��6D    �� 
IsReadOnly$
 True��6o   
���������6X   Z� ��6�   Z� .��6�   	Song Name52        54   
   -   5        $ 
artistName��54      $��2��6)   $��3��66   $��8��6F   $
 True��6q   
���������6Z   Z� ��6�   Z� .��6�   	Song Name56        57   
   -   5        $ 	albumName��57      $��3��6)   $��3��66   $��8��6F   $
 True��6q   
���������6Z   Z� ��6�   Z� .��6�   	Song Name59      �� 5:   
   -   5         ��Name$ 
linkButton��5:      $��10��6%   $��4��66   $��16��6t   $��22��6�   ��6D   Z� ��5;      �� 5<       �� ImageSource$ /Resources/Icons/play.png��6    j� Stretch$ Uniform=�6E    j� TileMode$
 None=�6W   5>      �� 5?   
   -   5        	 ��Name$
	 link��5?      $��3��6"   $��4��62   $��7��6@   $��Center=�6T   
 �� Padding$
 8,0,8,0q�6o   
��������� 5@      -   5         ��Name$ songHyperLink��5@      ���	#FF020202.�6t   .�5A      �� -	   5        $	 linkText��5A      
�������6-   5B      5C      �� 5E   
   -
   5         ��Name$ 	songImage��5E      $��1��6#   $��1��63   $��4��6@   6S   #  HandyControl.Controls.Divider  5F   
   $��2��6   $��6��6    $��0��6-   $��12��6=   
�������
�������$��0,9,0,-1q�6x    K� ZIndex$ 1��6�   
�������6�   5G      5H      