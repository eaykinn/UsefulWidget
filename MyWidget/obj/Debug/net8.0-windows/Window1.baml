   M S B A M L   `   `   `  ����C  ?MyWidget, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:MyWidgetMyWidget  5   	   e aMicrosoft.Web.WebView2.Wpf, Version=1.0.2903.40, Culture=neutral, PublicKeyToken=2a8ab48044d2601ekLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.WpfMicrosoft.Web.WebView2.Wpf 5   	       MyWidget.Window1   5   	   S OHandyControl, Version=3.5.1.0, Culture=neutral, PublicKeyToken=45be8712787a1e5b0hc'https://handyorg.github.io/handycontrol  W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   (localclr-namespace:MyWidget     5   	   \WpfLclr-namespace:Microsoft.Web.WebView2.Wpf;assembly=Microsoft.Web.WebView2.Wpf     5   	     � Title$  Similar Music List��5
   	   $	��550��6$   $	��456��61   $	��460��6S   $	��460��6b    � WindowStyle$
 None=�6�    � WindowStartupLocation$ CenterOwner=�6�    � 
ResizeMode$ NoResize=�6�   ��6=   Z� ��6q   Z� .��5      �  � RowDefinitions 5   
   �� 5      $��25 �6   �� 5      $��5 �6   �� 5      $��35 �6   �� 5      $��5 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $��25 �6   .+�5   
   %  HandyControl.Controls.SearchBar  -   5          Name$ songSearchBox��5      $��2��6/   
�������
������� �� 
CaretBrush �	#FFF5F2F2$��25,0,0,0q�6�   ��6<   Z� 6�   �� 5   
   -   5         ��Name$ songList��5      $��4��6$   $��0��61   $��2��6A   ��6U   Z� ��6k   Z� �� 5   
   -   5        $��0��5      $��1��6   $��17��6.   $��22��69   $��Bottom=�6F   	 K� ZIndex$	 1��6�   ��6v   Z� ��5      �� 5      
 �� ImageSource$ 
 /Resources/Icons/xmark.png��6   5      �� 5   
   -   5         ��Name$ toprect��5      $��0��6M   $��0��6]   $��2��6j   ��5       �� 5!       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5"      �  � Color$ Black��6#   � 5#      $ 	#FF085594��6#    � Offset$ 1Q�65   5$      5&      5(      5)      