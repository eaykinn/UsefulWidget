   M S B A M L   `   `   `  ���� C  ?MyWidget, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:MyWidgetMyWidget      MyWidget.songCard   S OHandyControl, Version=3.5.1.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     (localclr-namespace:MyWidget     $	��455��$	��120��.���   � RowDefinitions  �� $��10 ��� $	��20* ��� $	��20* ��� $	��20* ��� $	��20* ��� $��10 � � ColumnDefinitions �� $��10 ��� $
��150* ��� $	��70* ��� $	��40* ��� $	��40* ��� $	��40* ��� $	��40* ��� $	��40* ��� $	��40* ��� $	��40* ��� $	��40* ��� $��10 �.+��� -    ��Name$ songNameLbl��$��1��$��2��$��Center=�$
��Left=�
���������Z� ��Z� .��Song�� -   $ artistNameLbl��$��2��$��2��$
��Left=�
���������Z� ��Z� .��Artist�� $��3��$��2��$
��Left=�
���������Z� ��Z� .��Album�� $��4��$��2��$
��Left=�
���������Z� ��Z� .��Link#  HandyControl.Controls.TextBox  -     Name$ songName��$��1��$��3��$��8�� �� 
IsReadOnly$
 True��
���������Z� ��Z� .��	Song Name  -   $ 
artistName��$��2��$��3��$��8��$
 True��
���������Z� ��Z� .��	Song Name  -   $ 	albumName��$��3��$��3��$��8��$
 True��
���������Z� ��Z� .��	Song Name�� -    ��Name$ 
linkButton��$��10��$��4��$��16��$��22����Z� ����  �� ImageSource$ /Resources/Icons/play.png�� j� Stretch$ Uniform=� j� TileMode$
 None=��� -   	 ��Name$
	 link��$��3��$��4��$��7��$��Center=�
 �� Padding$
 8,0,8,0q�
��������� -    ��Name$ songHyperLink�����	#FF020202.�.��� -	   $	 linkText��
��������� -
    ��Name$ 	songImage��$��1��$��1��$��4��#  HandyControl.Controls.Divider  $��2��$��6��$��0��$��12��
�������
�������$��0,9,0,-1q� K� ZIndex$ 1��
�������